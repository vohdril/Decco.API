# 05 — Dossiê do STACK LEGADO real (OB.API / `bhi-ob-api`)

> Conhecimento profundo do stack **legado** da Omnibees, extraído por leitura direta de `C:\Git\bhi-ob-api` (somente leitura).
> Serve para (a) **entender** tecnologias legadas ainda usadas no mercado e (b) montar a **Decco.Legacy.API** fiel (ver `reference/07`).
> Tudo ancorado em `arquivo:linha`. É o **par oposto** do dossiê moderno em `reference/06`.

## Fotografia
.NET **Framework 4.8**, csproj **old-style** (MSBuild 2003) + `packages.config`. 55 projetos, 3 camadas físicas (SL/BL/DL) + DM (domínio) + módulos Unity. Host `SL\OB.REST.Services`. **Não há SDK-style, não há net8.0** — é legado puro (não em migração).

## Versões exatas (base para réplica fiel)
| Pacote | Versão | Nota |
|---|---|---|
| EntityFramework | **6.4.4** | database-first (EDMX); `EntityFramework.props/.targets` importados no csproj |
| Unity + Unity.Interception | **4.0.1** | ⚠️ namespace real = `Microsoft.Practices.Unity` (PublicKeyToken `6d32ff45e0ccc69f`) |
| Castle.Core | 5.1.1 | backing do interception |
| Dapper | **1.50.5** | SPs sobre a conexão do EF |
| Z.EntityFramework.Plus.EF6 | 1.8.23 | batch update/delete |
| ServiceStack.Redis / .Common / .Text | **3.9.71** | última linha 3.x BSD; **EOL de facto** |
| StackExchange.Redis | 2.1.58 | coexiste em paralelo |
| Newtonsoft.Json | 13.0.3 | serialização |
| Microsoft.Owin.* | 4.2.2 | Host.SystemWeb, Security, Security.OAuth |
| Microsoft.AspNet.WebApi.* | 5.3.0 | Web API 2 (Core/Owin/WebHost/Cors) |
| Asp.Versioning.WebApi | 7.1.0 | header `x-api-version` |
| NLog | 5.2.8 | logging (+ NLog.PciHideCC) |
| Swashbuckle / .Core | 5.6.0 | Swagger multi-versão |
| Consul | 1.7.14.2 | service discovery |
| OB.Api.Core | 1.1.1.17-stable | **binário externo** — contém as abstrações de infra (ver §5) |

Divergências de suposições comuns: **não usa IdentityServer3** (fonte vendored mas não referenciado); **não é NEST** (é `Elastic.Clients.Elasticsearch` 8.12); **Couchbase são DLLs vendored** (`Couchbase.NetClient` 2.7.4), não NuGet no host/DL.

## 1. Host — OWIN + Web API 2 (híbrido)
Host híbrido: **OWIN** (`Microsoft.Owin.Host.SystemWeb`) para auth + **`System.Web` classic pipeline** (Global.asax) para o Web API. Entrypoint `[assembly: OwinStartup(...)]` em `Startup.cs:10`; `Configuration(IAppBuilder app)` (`Startup.cs:16-43`) encadeia: `ConfigureAuth(app)` → `new HttpConfiguration()` (+ `NLogExceptionLogger`, `NLogControllerTraceHandler`, filtro global `ObTracingIdHandlerAttribute`) → `SwaggerConfigMultipleVersions.Register()` → `WebApiConfig.Register(config)` → `UnityConfig.RegisterComponents(config)` (**DI por último**).

- **Auth (`Startup.Auth.cs:34-54`):** OWIN **OAuth Bearer** puro (`app.UseOAuthBearerAuthentication(...)`) com `OnRequestToken`/`OnValidateIdentity` custom + **admin por subnet** (IPs permitidos recebem ticket Admin protegido por `IDataProtector`/`TicketDataFormat`) + `app.UseClaimsTransformation(CreateUser)` que injeta claims `wl_uid/wl_code/wl_type` e embrulha em `CustomPrincipal`.
- **Rotas/formatters (`WebApiConfig.cs`):** TLS 1.2 forçado; versionamento por header **`x-api-version`** (default v1); `MapHttpAttributeRoutes` + rota convencional `api/{controller}/{action}/{id}`; **CORS aberto** `EnableCorsAttribute("*","*","*")`; JSON Newtonsoft `DateTimeZoneHandling.Utc`, `Formatting.None`.
- **Global.asax.cs:** pipeline clássico paralelo (`AreaRegistration`, `GlobalConfiguration.Configure`, OpenTelemetry via `ObOpenTelemetry`), e remove headers `X-Powered-By`/`Server`.

> **Porquê é assim / o que teria hoje:** OWIN foi a ponte pré-`Microsoft.Extensions.Hosting`. Hoje isso tudo é o `WebApplication` builder do ASP.NET Core (host, DI, middleware num só pipeline). O híbrido OWIN+System.Web existe porque a app é `System.Web`-hosted (IIS) e OWIN entrou só para a auth.

## 2. DI + AOP — Unity 4 + Interception (o coração do legado)
Composição raiz em `UnityConfig.cs:31-88`: `new UnityContainer()` + duas `UnityContainerExtension` (`DataAccessLayerModule`, `BusinessLayerModule`) + `UnityResolver` ligado em `GlobalConfiguration.Configuration.DependencyResolver` e `config.DependencyResolver`. `UnityResolver : System.Web.Http.Dependencies.IDependencyResolver` (`GetService` engole `ResolutionFailedException`, `BeginScope` → child container).

**AOP (`BusinessLayerModule.cs`):** cada manager é registado por `RegisterPoco<TFrom,TTo>` com **`PerThreadLifetimeManager`** e `GetInterceptors()`:
```
new Interceptor<InterfaceInterceptor>(),
new InterceptionBehavior<LoggingInterceptionBehaviorAsync>(),   // 1º
new InterceptionBehavior<BusinessRulesInterceptionBehavior>()   // 2º
```
~100 managers assim. Cadeia: **LoggingAsync → BusinessRules → método real**.
- `LoggingInterceptionBehaviorAsync` — mede tempo (Stopwatch), trata `Task`/`Task<T>` via wrappers cacheados por reflexão, loga `MethodName`/`TimeTaken` e re-lança exceções.
- `BusinessRulesInterceptionBehavior` — **o envelope Request/Response**: pega `input.Inputs[0]` como `RequestBase`; se nulo, curto-circuita com `ResponseBase` de erro; após o método, propaga `RequestId` para o response e, em exceção, **`HandleException` traduz a exceção em `response.Errors`** (nunca 500 cru, exceto `UnauthorizedAccessException`/`NotImplementedException`). É AOP fazendo o trabalho que, no moderno, filtros/middleware fazem.

`DataAccessLayerModule.cs`: `AddNewExtension<Interception>()`, regista `IConfiguration`, o `ICacheProvider` **por reflexão** (§4), `ISessionFactory` (singleton sobre `DomainScopes.GetAll()`), `ITransactionManager` (`Mock` em DEBUG), `IRepositoryFactory`.

> **Porquê / hoje:** Unity + interception AOP = o jeito pré-DI-nativa de fazer cross-cutting (log, regras, transação) sem sujar o código. Hoje: DI nativa + middleware/filters + (se preciso) decorators via `Scrutor`. O `PerThreadLifetimeManager` reflete o modelo thread-por-request do `System.Web` (o Core é async/scoped).

## 3. Dados — EF6 database-first (EDMX) + Dapper híbrido
- **12 EDMX** (`OB.DL.Model.*`: BE, CRM, Channels, General, PMS, Payments, ProactiveActions, Properties, Rates, Reservations, SRM, OmnibeesHistory). Cada um gera um `DbContext` (T4) com `base("name=XContext")` e `OnModelCreating → throw new UnintentionalCodeFirstException()` (assinatura de database-first). POCOs vivem em `OB.Domain.*` com campo estático `public static readonly DomainScope DomainScope = DomainScopes.X;`.
- **Connection string EDMX:** `metadata=res://*/X.csdl|res://*/X.ssdl|res://*/X.msl;provider=System.Data.SqlClient;provider connection string="...MultipleActiveResultSets=True..."` com `providerName="System.Data.EntityClient"`. MARS é necessário para o Dapper coexistir.
- **`Repository<TEntity>`** (`Repository.cs:25-43`) — híbrido sobre a **mesma conexão**: `_objectSet = ctx.Context.Set<TEntity>()` (LINQ) e `_connection = ctx.Context.Database.Connection` (ADO, aberta no ctor).
- **`RepositoryFactory`** resolve o contexto por **scope + reflexão** (`GetField("DomainScope")`; `FindContext` casa `"BEContext".Split("Context")[0] == scope.Name`).
- **Stored procedures via Dapper** sobre `_connection`: `Query<QR>/QueryAsync<QR>/Execute("NomeDaSP", DynamicParameters, commandType: CommandType.StoredProcedure)`; TVPs via `DataTable`+`DbType.Object`; `SqlBulkCopy` sobre `(SqlConnection)_connection`. 22 repos em `Repositories\Impl\SqlServer\`. Resultados em POCOs planos `QueryResultObjects` sufixo **`QR1`** (117 ficheiros).

> **Porquê / hoje:** database-first via EDMX (XML `.csdl/.ssdl/.msl` + designer do Visual Studio) foi o modelo dominante do EF ~2008-2016. Hoje: EF Core (code-first + migrations, ou `dotnet ef dbcontext scaffold` para database-first sem EDMX). O híbrido EF+Dapper para SP **continua atual** (EF Core faz o mesmo com `Database.GetDbConnection()`).

## 4. Cache — `ICacheProvider` + provider por reflexão
Interface `ICacheProvider` (Get/Set/Add/Replace/Remove/Increment/Decrement/**AcquireLock**/`Set(key,CacheEntry)`/`Invalidate`). Implementação `ServiceStackCacheProvider` (`PooledRedisClientManager`, read/write/failover hosts, `MaxReadPoolSize=50`, `FailoverTo(...)`, locks distribuídos). **Selecionada por reflexão** em `DataAccessLayerModule.cs:63-81`: `Assembly.GetType(Configuration.CacheProvider)` (nome-do-tipo na config) → registada singleton; fallback `DefaultCacheProvider` (in-process). ⚠️ `ServiceStack.Redis 3.9.71` é EOL; `Invalidate<TRepo>` é `NotImplementedException`.

> **Porquê / hoje:** a indireção "provider por nome-de-tipo na config" é um strategy pattern manual — elegante e ainda válido. O que muda hoje é o backend: StackExchange.Redis/FusionCache no lugar do ServiceStack.Redis 3.x.

## 5. Abstrações que vêm do binário externo `OB.Api.Core` (o "buraco" da réplica)
`IObjectContext` (`.Context`→`DbContext`), `ISessionFactory` (`GetUnitOfWork(scope)`), `IUnitOfWork`/`UnitOfWorkBase`, `DomainScope` (+ enum `DomainScopeType`) — **não estão no código-fonte**; o legado só as **consome** (`using OB.Api.Core;`). Uma réplica fiel (`Decco.Legacy.API`) **precisa reimplementá-las do zero**.

## 6. WCF — client-only (importante)
O OB.API **não hospeda WCF**: zero `.svc`, zero `[ServiceContract]` de servidor, `<system.serviceModel>` só com `<bindings>`+`<client>` (sem `<services>`). O que existe é **cliente SOAP gerado** (`HtmlTemplateService`, proxy `ClientBase<T>`, `basicHttpBinding`, endpoint `.asmx` no `<client>`), chamado em `BeManagerPoco`/`PropertyManagerPoco`. → O "WCF do legado" a estudar é **consumo** (service reference, proxy, binding no config), **nunca hosting**.

## Mapa "porquê ainda importa no mercado"
EF6, Unity/AOP, Web API 2/OWIN, WCF-client e .NET Framework 4.8 **ainda rodam** em muitíssimos sistemas corporativos em manutenção. Saber lê-los e mantê-los é competência empregável — é o valor da `Decco.Legacy.API`. O par moderno (`reference/06`) mostra para onde tudo isso evoluiu.
