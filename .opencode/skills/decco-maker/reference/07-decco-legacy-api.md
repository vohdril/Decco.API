# 07 — Blueprint da **Decco.Legacy.API** (réplica FIEL do legado OB.API)

> A `Decco.Legacy.API` é o **terceiro alvo** do sandbox: uma versão **não modernizada** do core, fiel ao stack do OB.API
> (`reference/05`), sobre o **mesmo DeccoDB**. Objetivo: aprender/manter tecnologias legadas ainda usadas no mercado
> (EF6/EDMX, Unity+AOP, Web API 2/OWIN, WCF-client, .NET Framework). Compara-se lado a lado com a **Decco.API** (moderna),
> que faz o **mesmo papel** em tecnologia atual. **Nada de código aqui — é o mapa.**

## Papel e relação com os outros alvos
| Alvo | Stack | Papel | Prioridade agora |
|---|---|---|---|
| **Decco.API** | **moderno** (.NET 8, EF Core+Dapper, DI nativa) | core dono do DeccoDB | 🔴 foco |
| **Decco.Legacy.API** | **legado** (.NET Fx 4.8, EF6/EDMX, Unity/AOP, OWIN/WebApi2) | **mesmo** core, sobre o mesmo DeccoDB | 🟠 foco de estudo do legado |
| **Foundation.API** | moderno (fachada Partners-like) | consumidor HTTP | ⚪ **despriorizada** (Paulo já domina Partners) |

> A dupla **Decco.API × Decco.Legacy.API** é o exercício central: *o mesmo problema, dois mundos*. Onde a moderna usa
> `AddDbContext`+DI nativa, a legada usa EDMX+Unity; onde a moderna usa `IDistributedCache`, a legada usa `ICacheProvider`+ServiceStack.Redis.

## Stack-alvo (fiel, com versões de `reference/05`)
.NET **Framework 4.8**; csproj **old-style + packages.config**; **EF6 6.4.4** (database-first/EDMX); **Dapper 1.50.5**; **Unity 4.0.1** (`Microsoft.Practices.Unity`) + **Unity.Interception**; **Microsoft.Owin.* 4.2.2** + **Microsoft.AspNet.WebApi.* 5.3.0**; **ServiceStack.Redis 3.9.71** (⚠️ EOL — ver nota); **Newtonsoft.Json 13.0.3**; **NLog 5.2.8**; **Swashbuckle 5.6.0**; versionamento por header `x-api-version`.

## Layout de solução (espelha o OB.API, enxuto para o DeccoDB)
```
Decco.Legacy.sln
├─ SL\ Decco.Legacy.REST            (host OWIN + Web API 2; Global.asax; App_Start: Startup(.Auth), WebApiConfig, UnityConfig, UnityResolver)
├─ BL\ Decco.Legacy.Contracts       (o ENVELOPE: RequestBase/ResponseBase, Single/Bulk/Paged, Status/Errors/Warnings/RequestId — DataContract)
├─ BL\ Decco.Legacy.Operations      (managers {X}ManagerPoco : BusinessPocoBase + BusinessLayerModule + os 2 interception behaviors)
├─ DL\ Decco.Legacy.Common          (Repository<T>, RepositoryFactory, DataAccessLayerModule, ICacheProvider + ServiceStackCacheProvider, QueryResultObjects\*QR)
├─ DL\ Decco.Legacy.Model.Decco     (EDMX do DeccoDB → DeccoContext : DbContext)
├─ DM\ Decco.Legacy.Domain          (POCOs do EDMX + campo estático DomainScope)
└─ DL\ Decco.Legacy.Core            (⭐ REIMPLEMENTAR: IObjectContext, ISessionFactory, IUnitOfWork/UnitOfWorkBase, DomainScope — ver §"buraco")
```

## O "buraco" a preencher (⭐ o principal trabalho da réplica)
No OB.API, `IObjectContext`/`ISessionFactory`/`IUnitOfWork`/`UnitOfWorkBase`/`DomainScope` vêm do **binário externo `OB.Api.Core`**.

> ⚠️ **Atualização (2026-07-13):** o fonte do `OB.Api.Core` foi disponibilizado e está **dissecado em `reference/08`**. Continua a
> **reimplementar-se** (o sandbox não depende do pacote corporativo), mas agora **com o molde real** — assinaturas exatas de
> `IObjectContext`/`ISessionFactory`/`UnitOfWorkBase`/`DomainScope`, o padrão nested-scope `AsyncLocal`+shell, a tradução
> `DataLayerException` e a peça de escala `DatabaseDiscovery/RoundRobin`. Ver `knowledge-drops/005`.

A `Decco.Legacy.API` **precisa reimplementá-los** (projeto `Decco.Legacy.Core`) — ver o molde em `reference/08`:
- `DomainScope` — nome + nome-do-contexto (`"Decco"`→`"DeccoContext"`/`"DeccoContextReadOnly"`) + tipo.
- `IObjectContext` — expõe `.Context` (o `DbContext` EF6).
- `IUnitOfWork`/`UnitOfWorkBase` — abre/mantém contexto(s) por scope; `Save()`.
- `ISessionFactory` — `GetUnitOfWork(scope)`.
- `ITransactionManager` — `BeginTransactionScope(scope)` (`TransactionScope`).

> Como o DeccoDB é **um** banco, dá para começar com **um único scope** (`Decco`) e um contexto — simplificação legítima do multi-scope do OB.API (que tinha 12).

## Camadas — como cada peça mapeia ao DeccoDB
1. **DL/EDMX:** gerar o EDMX do DeccoDB (designer do Visual Studio → "EF Designer from database"). Gera `DeccoContext : DbContext` (`base("name=DeccoContext")`, `OnModelCreating → UnintentionalCodeFirstException`) + POCOs (`Anomalia`, `Cat_*`, `EntidadeViva`…) no `Decco.Legacy.Domain`. Connection string EDMX `metadata=res://*/Decco.csdl|...ssdl|...msl;provider=System.Data.SqlClient;...MultipleActiveResultSets=True` (creds fictícias/placeholder).
2. **DL/Repositório híbrido:** `Repository<T>` guarda `_objectSet = ctx.Set<T>()` e `_connection = ctx.Database.Connection` (abre no ctor). **CRUD por LINQ; as `sp_Anomalia_*` por Dapper** (`_connection.Query<AnomaliaQR1>("sp_Anomalia_Buscar", DynamicParameters, commandType: CommandType.StoredProcedure)`). POCOs de SP em `QueryResultObjects` sufixo `QR`. `sp_Anomalia_Buscar` já devolve página + `TotalRegistros` → encaixa no `PagedResponse`.
3. **BL/Managers + AOP:** `AnomaliaManagerPoco : BusinessPocoBase` (usa `RepositoryFactory`/`SessionFactory`/`TransactionManager`). Registo por `RegisterPoco<IAnomaliaManagerPoco,AnomaliaManagerPoco>` com `PerThreadLifetimeManager` + `Interceptor<InterfaceInterceptor>` + `LoggingInterceptionBehaviorAsync` + `BusinessRulesInterceptionBehavior` (este converte exceção→`response.Errors`).
4. **SL/Host:** OWIN `Startup` + Web API 2 `WebApiConfig` (rota `api/{controller}/{action}/{id}`, header `x-api-version`, Newtonsoft UTC) + `UnityConfig`+`UnityResolver`. Controllers finos `[AcceptVerbs("POST")]` recebendo `RequestBase`, delegando ao manager. Auth: começar com OWIN OAuth Bearer simplificado (ou `[AllowAnonymous]` no sandbox) — o admin-por-subnet é opcional.
5. **DL/Cache:** `ICacheProvider` + `ServiceStackCacheProvider` (pool, read/write/failover, locks) selecionado **por reflexão** (`Assembly.GetType(Configuration.CacheProvider)`), fallback in-process. Cachear os `Cat_*` (read-only).
6. **WCF (opcional, client-only):** para tocar em WCF, **consumir** um SOAP externo (ex.: um serviço ASMX stub que devolva um "template de relatório de anomalia"): service reference → proxy `ClientBase<T>` → `<client><endpoint>`+`basicHttpBinding` no `Web.config`. **Nunca hospedar** (o legado não hospeda).

## O que MANTER vs DROPAR (fidelidade com bom senso de sandbox)
| Manter (é o ponto do exercício) | Dropar (ruído para o estudo) |
|---|---|
| .NET Framework 4.8, csproj old-style + packages.config | NServiceBus/MSMQ, RazorEngine |
| EF6/EDMX database-first + `UnintentionalCodeFirstException` | Consul (service discovery) |
| Repository híbrido EF6 + **Dapper SP** + `QR` POCOs | Couchbase, Elasticsearch |
| Unity 4 + **Interception AOP** (2 behaviors) + `UnityResolver` | Vault (usar placeholder/appSettings no sandbox) |
| Envelope Request/Response (DataContract) | OpenTelemetry (opcional) |
| `ICacheProvider` + provider por reflexão | admin-por-subnet (simplificar auth) |
| OWIN + Web API 2, header `x-api-version` | os 12 scopes → usar **1** (`Decco`) |
| WCF **client** (se quiser tocar em WCF) | WCF hosting (não existe no legado) |

## ⚠️ Nota EOL / ferramental (ler antes de gerar)
- **ServiceStack.Redis 3.9.71** é EOL/BSD-antigo. Para estudo, fixar a versão e assinalar; **não** é o que se usaria em produção (lá seria StackExchange.Redis/FusionCache — ver `reference/06`). Alternativa fiel-mas-viva: manter a **forma** (`ICacheProvider`+pool+failover) com StackExchange.Redis por baixo.
- **EDMX exige Visual Studio** (designer). Alternativas se não houver VS: gerar as classes EDMX à mão (trabalhoso) ou aceitar EF6 **code-first** como aproximação (perde o `.edmx`, mantém EF6). Ver `open-questions/` (Q sobre ferramental legado).
- **.NET Framework 4.8 Developer Pack** necessário para compilar; roda em Windows (o Paulo está em Windows 11 ✔).

## Trilha de estudo (comparativa)
Fazer a **mesma** entidade `Anomalia` nas duas: primeiro na **Decco.API** (moderna) — rápido, familiar — depois na **Decco.Legacy.API** (legada) e **comparar linha a linha**: `AddDbContext` vs EDMX; DI nativa vs Unity+AOP; `IDistributedCache` vs `ICacheProvider`; middleware/filtros vs interception behaviors; `WebApplication` vs OWIN+Global.asax. O *delta* é o aprendizado.
