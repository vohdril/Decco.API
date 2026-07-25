# reference/03 — O envelope, o fio HTTP e as camadas da fachada

> Fonte: `OB.BL.Contracts` (envelope) + `partners-api` (fachada, conversores, FusionCache, consumo HTTP).

## O envelope (Decco.Contracts) — a linguagem do core

Reproduzir a **forma** (não o `DataContract`/WCF do OB.API):
- `RequestBase { RequestId }` (correlação) · `ResponseBase { Status, Errors[], Warnings[] }` com `Status ∈ {Success, PartialSuccess, Fail}`.
- Escrita: `Insert/Update{X}Request { Item, IncludeResult }` → `{X}Response { Result }`.
- Leitura: `List{X}Request { PageIndex (0-based), PageSize, ReturnTotal, Include* }` → `List{X}Response { Result[], TotalRecords }`.

> **Molde fiel + forma enxuta (ver `reference/10`):** o fonte real do envelope (`OB.BL.Contracts`) está dissecado em `reference/10`.
> Siga a **forma enxuta do `OB.PAR.Contracts`** — concretos herdam **direto** de `RequestBase`/`ResponseBase` (`Single`/`Bulk` só
> quando o caso pedir; **sem** as props `[Obsolete]` do monólito). No Decco moderno, **System.Text.Json** no lugar do `DataContract`/WCF.

**Porquê um envelope (e não REST puro no core):** dá **correlação** (`RequestId`), **erros/warnings estruturados** e um
protocolo **uniforme** para toda operação — é o que permite o helper genérico `CallApiAsync<TReq,TResp>` na fachada.
*Alternativa rejeitada:* cada endpoint com seu shape ad-hoc — a fachada não conseguiria um transporte genérico.

## O fio (Foundation → Decco.API)
- Sempre `POST /api/{Controller}/{Action}`, JSON.
- Auth de serviço `client_credentials` (token cacheado, margem −5min) + header de identidade.
- `Status==Fail` ou `Errors` não-vazio → a fachada lança `ApiException` (traduz `Error` do envelope → `Error` do DTO).
- **Paginação:** fachada pública 1-based → core 0-based (`PageIndex = page-1`); `HasNextPage = (PageIndex+1)*PageSize < TotalRecords`.

## Camadas da fachada (Foundation.API) — os 6 artefatos por entidade

Ordem de dependência: **1** contratos DTO V2 → **2** validadores → **3** conversores Receive/List → **4** repositório Remake →
**5** serviço (envelope) → **6** controller V2.

1. **Contratos DTO V2** (`Foundation.Dtos/V2/{Entidade}`): as **13 classes** (Create/Update/Get/Delete/List × Parameters/Request/Response) + modelo-raiz `{Entidade}` (`{code}`, `Configuration`) + `{Entidade}Configuration` (miolo editável) + Enums. Campos quase sempre `string`; `[OpenApi*]` só documenta Swagger.
2. **Validadores** (FluentValidation): Request (`Cascade(Stop).NotNull().SetValidator(ConfigurationValidator)`), Configuration (campos), Parameters (código).
3. **Conversores** `Receive/` (DTO→envelope, delta-merge no Update via `OldObject`) e `List/` (envelope→DTO, 4 peças compostas por `IConverterProvider`). Base bidirecional; sentido inverso lança `NotImplementedException`.
4. **Repositório Remake** (`Foundation.Api.Infrastructure`): **FusionCache** (L1+L2+backplane, fail-safe, eager-refresh 80%, soft/hard timeout, **não cacheia null**) + transporte HTTP `CallApiAsync`. Como o DeccoDB não tem multi-tenant, usar a variante **global** (não hotel-scoped).
5. **Serviço** (o envelope de método): `try` (validar → montar Info + dicionários → converter → repositório) · **3 catches** (erro interno / `ApiException` / genérico) · `finally` (logs). Devolve o response.
6. **Controller V2**: 1 action/operação, `[FromRoute]`/`[FromQuery]` (Parameters) e `[FromBody]` (Request) separados; delega e devolve `GeneralResponseTreatment(response[, 201])` (sem erro→200/201; NotFound→404; DefaultError→500; resto→400).

**Porquê a fachada é tão mais rica que o core:** o core só move dados; a fachada **protege** (validação, auth), **acelera**
(cache) e **traduz** (DTO público ↔ envelope, Id ↔ código). *E se a fachada não existisse?* O cliente falaria o envelope
cru e Ids internos — acoplado ao core e sem cache/validação de borda.

## FusionCache no Decco (resumo — detalhe conceitual pode vir da análise de Redis)
L1 memória (ns) + L2 Redis (compartilhado) + backplane (Pub/Sub invalida L1 entre instâncias). Fail-safe serve *stale* se o
core cair; eager-refresh a 80% do TTL evita o "cliff"; soft/hard timeout impede que core lento vire cliente lento.

## Fio a puxar 🔎
O `RequestId` do envelope é a mesma ideia do "correlation id" de observabilidade. Como você propagaria esse id da fachada
para o core e para os logs, de ponta a ponta? (Conecta com a parte de observabilidade/Grafana que já estudamos.)
