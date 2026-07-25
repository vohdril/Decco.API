# reference/01 — Papéis, topologia e nomenclatura canônica

> Fonte: análise dos repos `C:\Git\{bhi-ob-api,partners-api,partners-dtos}` + `decco.sql`. Espelha a relação Omnibees.

## Papéis (decorar isto primeiro)

| Sandbox | Espelha | Papel |
|---|---|---|
| **Decco.API** | **OB.API** (`bhi-ob-api`) | **CORE**: possui e conecta ao **DeccoDB**; expõe operações via **envelope** |
| **Foundation.API** | **Conector Partners** (`partners-api`) | **FACHADA** em camadas; **não toca no banco**; consome a Decco.API por HTTP |
| **Foundation.Dtos** | `Partners.Dtos` | contratos públicos **V2** (POCO) que a fachada expõe |
| **Decco.Contracts** | `OB.BL.Contracts` | o **envelope** Request/Response que trafega no fio (referenciado pela fachada) |

**Porquê os nomes assim:** o banco é **DeccoDB** → **Decco.API** é seu dono (core). *E se invertesse?* Voltaria a ser a
confusão da v1 do blueprint — e quebraria a intuição "quem tem o banco tem o nome do banco". **Alternativa rejeitada:** um
só serviço fazendo tudo — perderia a lição central (separar core de fachada, que é o que a Omnibees faz).

## Topologia

```
Foundation.API (FACHADA) ── Controllers V2 → Services+Converters → Repos(FusionCache) ──HTTP──┐
   Foundation.Dtos (público)  ◄─ traduz ─►  Decco.Contracts (envelope no fio)                 │
Decco.API (CORE) ◄── Controllers(envelope) → Managers → EF Core + Dapper(SP) ────────────────┘
                                                          └── DeccoDB (SQL Server)
```

## Nomenclatura dos projetos (usar EXATAMENTE estes nomes ao gerar)

**Solução `Decco.sln` (core):**
- `Decco.Api.REST` — SL: host ASP.NET Core; controllers `POST /api/{Entidade}/{Op}`; Swagger; DI.
- `Decco.Api.Operations` — BL: `{X}Manager` (regras + orquestração).
- `Decco.Contracts` — o envelope (`RequestBase`/`ResponseBase`, Single/Bulk/Paged) + DTOs de dados (`Anomalia`, `Cat*`…). **Referenciado pela Foundation.**
- `Decco.Api.DataLayer` — DL: `DeccoDbContext` (EF Core) + repositórios híbridos (EF+Dapper) + `ICacheProvider`.

**Solução `Foundation.sln` (fachada):**
- `Foundation.Api.REST` (SL) · `Foundation.Api.Root` (SL: CompositionRoot) · `Foundation.Dtos` (público V2) ·
  `Foundation.Api.Contracts` (error codes) · `Foundation.Api.Contracts.Validation` (FluentValidation) ·
  `Foundation.Api.Services` (serviços + conversores) · `Foundation.Api.Infrastructure` (FusionCache + HTTP à Decco.API; referencia `Decco.Contracts`) · `Foundation.Api.Common` (settings/helpers).

**Porquê 4 projetos no core e 8 na fachada:** o core é simples (banco + operações), a fachada é onde mora a complexidade de
tradução/validação/cache (é o que a Partners tem 9 projetos para fazer). *E se juntasse tudo num projeto?* Funciona no
começo, mas some a lição de **fronteiras de camada** — que é metade do valor do exercício.

## Endpoints (contrato de superfície)
- **Foundation.API** (público, REST V2): `GET/POST/PUT/DELETE /api/anomalias`, `/api/anomalias/{code}`.
- **Decco.API** (core, envelope): `POST /api/Anomalias/{ListAnomalias|GetAnomalia|InsertAnomalia|UpdateAnomalia|DeleteAnomalia}`, `POST /api/Catalogos/List*`.

## Fio a puxar 🔎
Antes de gerar: decida se **Decco.Contracts** (o envelope) será um projeto compartilhado por referência de projeto, ou um
**pacote** (como `Partners.Dtos`/`OB.BL.Contracts` são NuGets). Qual reflete melhor a realidade Omnibees? O que muda no acoplamento? (Registre a decisão em `knowledge-drops/`.)
