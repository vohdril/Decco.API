# 004 — Sem front-end acoplado + mapa de disponibilidade dos fontes externos

- **Data de registo:** 2026-07-12
- **Fonte:** levantamento read-only nos 3 repos (front-end + inventário de binários internos/vendored e busca de fontes no disco)
- **Tipo:** conhecimento resolvido (factual) — afeta o que a Decco.Legacy.API precisa reimplementar
- **Afeta:** reference/05, reference/07 (o "buraco" a reimplementar), escopo do sandbox (backend-only)

> ⚠️ **Atualização (2026-07-13, ver `knowledge-drops/005`):** o **Achado B** abaixo (mapa de fontes) mudou — os fontes de
> **`OB.Api.Core`** e de **`OB.Api.Base.*` (parcial)**, antes "só-DLL", foram disponibilizados (`OneDrive\...\Omnibees\*.zip`) e
> estão dissecados em `reference/08`/`09`/`10`. O texto abaixo mantém-se como o estado *daquele* momento. O **Achado A**
> (os 3 repos são backend-only, sem front-end) **continua válido**.

## Achado A — NENHUM dos três repos acopla front-end (todos backend-only)
- **bhi-ob-api (OB.API):** API-only. Os "sinais" NÃO são UI de produto: (1) `.cshtml` só da **HelpPage** (docs automáticas da Web API) em `SL\...\Areas\HelpPage\Views`; (2) **RazorEngine 3.10** gera **string HTML server-side** (templates de conteúdo/e-mail) — `OB.BL.Operations.Common\Helper\Template Manager\TemplateEngine.cs`, não serve páginas; (3) ASMX remoto `HtmlTemplateService` devolve **string** de template; (4) `Main\Clients\OB.API.Clients.Angular` é **SDK-cliente gerado por nswag** (npm `ob-api-clients`), **fora de qualquer .sln**, não hospedado.
- **partners-api:** API-only (único projeto web = `Partners.Api.REST` com `Sdk.Web`; zero `.cshtml/.ts/wwwroot/Views`).
- **partners-dtos:** biblioteca de DTOs, sem UI.

**Implicação para o Decco:** o sandbox é **backend-only** por natureza — nada de front-end a replicar. Se um dia quiser UI, é **adição nova** (Swagger UI já vem; um SPA seria projeto à parte). Templating server-side (RazorEngine/ASMX) é exercício **opcional** e **não** é "front-end".

## Achado B — Mapa de fontes externas (o que está no disco vs só binário)
**Feed dos pacotes internos:** `tfs.omnibees.com` (Azure DevOps/TFS Artifacts) — declarado em `partners-api\nuget.config` (feed `tfsOmnibees`). Sem credenciais inline em nenhum nuget.config. `bhi-ob-api` não tem nuget.config no repo (restaura do global). Feed local do usuário: `C:\localNugets`.

**Fonte DISPONÍVEL no disco:**
- **Partners.Dtos** → fonte completa em `C:\Git\partners-dtos` (origem do pacote `1.0.0.132`; nupkgs em `C:\localNugets`).
- **Envelope legado (`OB.BL.Contracts`)** → **projeto in-tree** em `C:\Git\bhi-ob-api\Main\BL\OB.BL.Contracts` (⭐ dá para LER o fonte real do envelope `RequestBase`/`ResponseBase`/Single/Bulk/Paged em vez de inferir do uso — usar como referência ao modelar `Decco.Contracts` / `Decco.Legacy.Contracts`). Nota: pode divergir da versão publicada `1.2.0.269`.
- **`OB.Log`** → projeto in-tree em `C:\Git\bhi-ob-api\Main\DL\OB.Log` (legado; pode divergir do pacote publicado).

**Fonte AUSENTE (só DLL; teria de ser clonada do feed tfs.omnibees.com):**
- ⭐ **`OB.Api.Core`** (as abstrações `IObjectContext`/`ISessionFactory`/`IUnitOfWork`/`UnitOfWorkBase`/`DomainScope` que a **Decco.Legacy.API precisa REIMPLEMENTAR**) — **confirmado: não há fonte no disco**, só a DLL em `packages\`. Isto **confirma** a decisão de `reference/07`: reimplementar do zero (não há como copiar).
- Toda a família **`OB.Api.Base.*`** (base moderna, 1.0.0.195+) — só DLL. → para a **Decco.API moderna**, reimplementar o mínimo necessário (DI por convenção, `IObHttpClient`, base de cache, base de conversores) em vez de copiar.
- Também só-DLL: `OB.Api.Core.Tracing`, `OB.ServiceDiscovery`, `OB.ClaimsTransformation`, `OB.Events.*`, `OB.PAR.Contracts`, `OB.Reservation.BL.Contracts`, `OB.PaymentGateways`, `OB.Security`, `OB.Services.Jobs.Operations`, `OB.TripAdvisor`.
- **DLLs vendored** (`Main\Assemblies\`, sem fonte): `Couchbase.Linq`, `Couchbase.NetClient`, `ODS.Messages`, `ODS.Queue.Worker.Coordinator.Client`, `OpenTracing(.Signed)`, `Remotion.Linq`.

`C:\Git\ob-skills` está vazio; `C:\Users\paulo.araujo\source\repos` vazio; nenhum clone de `OB.Api.Core`/`OB.Api.Base` no disco.

## Como aplicar
- Ao modelar o **envelope** (`Decco.Contracts`/`Decco.Legacy.Contracts`): **ler o fonte real** em `bhi-ob-api\Main\BL\OB.BL.Contracts` (disponível!) — fidelidade máxima.
- Ao construir a **Decco.Legacy.API**: **reimplementar** `OB.Api.Core` (sem fonte) — confirma `reference/07` §"buraco".
- Ao construir a **Decco.API moderna**: **reimplementar** o mínimo do `OB.Api.Base.*` (sem fonte); não depender dos pacotes internos Omnibees (não fazem parte do sandbox e exigiriam o feed corporativo).
- **Não** tentar restaurar pacotes `OB.*` do `tfs.omnibees.com` no sandbox (feed corporativo, fora do escopo pessoal/isolado).

## Fio a puxar
Vale clonar os repos-fonte de `OB.Api.Core`/`OB.Api.Base` do Azure DevOps para *estudar* (não para depender)? É opcional e depende de acesso ao TFS — registar como decisão do Paulo se ele quiser.
