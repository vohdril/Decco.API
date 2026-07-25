# 09 — Fonte REAL do `OB.Api.Base.*` (base moderna) — o que a Decco.API (moderno) reaproveita

> Vieram 3 projetos como fonte: **`OB.Api.Base.Abstractions`**, **`OB.Api.Base.DataLayer`**, **`OB.Api.Base.DataLayer.EF`**
> (EF Core **8.0.10**). O resto da família é **só-DLL** (§4). Dissecado read-only (`scratchpad/omni-src/OB.Api.Base.*`). É o análogo
> moderno do `OB.Api.Core` (`reference/08`): mesma ideia de UoW/SessionFactory/repositório, em EF Core + DI nativa.

## 1. Abstractions — contratos puros (zero implementação)
- **Http** `IObHttpClient`: `AddDefaultHeader`, `SetBaseAddress`, `SendAsync/GetAsync/PostAsync`, e os overloads-chave `GetJsonAsync<T>` / `PostJsonAsync<T>(uri, obj, opts, ct, onError)` (System.Text.Json embutido). A **implementação concreta NÃO veio** (só o contrato).
- **Auth**: só `IAuthUserBasicInfo` (`GetUserId`/`GetUsername`, p/ logging). **Não** há `IAuthTokenProvider`/`IAuthUserProvider` aqui.
- **Helpers (motor de conversão — só interfaces):** `IConverter<T1,T2>` (bidirecional, com `HashSet<Type> includes`), `IMapper<T1,T2>` (in-place sobre destino existente), `IConverterMapper<T1,T2>` (união), `IConverterProvider` (service-locator: `GetConverter/GetMapper/GetConverterMapper<T1,T2>()` + atalhos `Convert/Map`), `IHelperFactory` (fachada: `DateTimeProvider`/`ConverterProvider`/`RandomProvider`/`MediaFileManager`). ⚠️ **`AConverterMapperBase` NÃO existe no moderno** — é padrão do **legado** (`OB.DL.Common`). No moderno o motor é **provider-based**, e a classe-motor concreta **não veio**.
- **Logging/Tracing**: `IEnrichedProperties` (bag p/ Serilog), `ITracingIdAccessor` (`GetTracingId`/`GetIsRecording`).

## 2. DataLayer — repositório/critério agnóstico de persistência
- **`IRepository : IDisposable`** = marker vazio; **`IRepositoryFactory.GetRepository<TRepo>()`** resolve via `IServiceProvider`. (Não há CRUD genérico aqui — só na variante SQL.)
- ⭐ **Criteria (o núcleo de valor, copiável quase verbatim):** `ACriteriaBase` → `AQueryableCriteriaBase` com `PageIndex/PageSize/ReturnTotal/TotalRecords`, `Fields` (projeção, merge com `DefaultFields`+`Distinct`), `Orders` (cai em `DefaultOrders`), `StateFilter`, `Filter`, `GetAllItems`; hooks abstratos `IsAnyFilterExtended()`/`DefaultFields`/`DefaultOrders`. Suporte: `FilterInfo` (recursivo: `Field`/`Operator`/`Conjunction`/`Value`/`Filters`), `FilterOperator` (17 valores: Contains/StartsWith/IsNull/…), `FilterConjunction`, `SortByInfo`/`SortDirection`, `ProjectField` (suporta `"Relacao.Campo"`), `StateFilter` (Active/Inactive/Deleted).
- **Bases de repositório:** `ARepositoryBase` (logger tipado + Dispose), `AQueryableRepositoryBase<T>` (motor de query: `ApplyPagedCriteriaAsync` traduz Criteria→`DataSourceRequest` do pacote **`OB.DynamicLinqCore`** e chama `ToDataSourceResult`), `AHttpRepositoryBase` (injeta `IObHttpClient`), `ACachedRepositoryBase<T>`/`ACachedFallbackHttpRepositoryBase` (injeta `ICacheProvider`, `BuildCacheKey`).
- **Auto-registo por convenção:** `AddObDataLayerModule` (regista `ICacheProvider`→`GenericCacheProvider` + `IRepositoryFactory` Scoped); `RegisterRepositoriesAutomatically` (varre assemblies, filtra `System.*/Microsoft.*/*.Test/...`, acha classes `IRepository` e regista as interfaces-pai; default **Transient**).

## 3. DataLayer.EF — concretização EF Core 8
- **`IUnitOfWork`** (`BeginTransactionAsync`/`Commit`/`Rollback`, `SaveChangesAsync`, `IsReadOnly`), **`ISqlRepository<TEntity>`** (CRUD **tracked**: Add/Update/Attach/Remove single+bulk — leitura vem do `AQueryableRepositoryBase`), **`IDbObjectContext`** (embrulha `DbContext` + hooks `OnSavingChangesAsync`/`OnSavedChangesAsync`), **`ISessionFactory.GetUnitOfWork(bool isReadOnly)`**, **`ADbDomainBase`** (marker das entidades de BD).
- **Infra:** `UnitOfWork<TContext>` (orquestra os hooks no `SaveChangesAsync`; `ConcurrentBag<IDbObjectContext>`), `UnitOfWorkShell<TContext>` (**aninhamento** — não dispõe a original), `SessionFactory<TContext>` (sob lock: se há UoW corrente devolve shell, senão cria; impede trocar `isReadOnly` no meio).
- ⭐ **`SqlRepositoryBase<TEntity>`** = `AQueryableRepositoryBase<T>` + `ISqlRepository<T>`: `DbSet` lazy; `GetQuery` usa `AsNoTrackingWithIdentityResolution()` quando readonly; valida invariantes (≥1 filtro **ou** `GetAllItems`); `ApplyPagedCriteriaAsync` override com `ToListAsync`; **NÃO faz `SaveChanges`** (é da UoW). CRUD chama `ThrowExceptionIfIsReadonly()`.
- **Auto-registo EF:** `AddObDataLayerEFModule<TContext>()` (regista `SessionFactory<TContext>` Scoped + `IDbObjectContext` Transient-via-factory que se auto-adiciona à bag da UoW); `RegisterGenericSqlRepositoriesAutomatically` (acha `ADbDomainBase` e regista `ISqlRepository<Model>`→`SqlRepositoryBase<Model>`).

## 4. O que NÃO veio (só-DLL / ausente)
`OB.Api.Base.DI.AspNetCore(.Caching/.HealthChecks/.Tracing/.RateLimit/.Metrics)`, `OB.Api.Base.BusinessLayer`, `OB.Api.Base.Contracts(.Validation)`, `OB.Api.Base.Helpers` (concreto). Também **ausentes**: a impl de `IObHttpClient` (o `ObHttpClient`), o **motor concreto de conversores** (`ConverterProvider`/`HelperFactory`) e as impls de Auth/Logging/Tracing. → o `ICacheProvider` (usado pelas bases `ACached*`) vem de `OB.Api.Base.DI.AspNetCore.Caching` = **só-DLL**.

## 5. Mínimo copiável p/ um vertical slice moderno (Decco.API)
Copiar quase verbatim: **Criteria** (§2) + **UoW/SessionFactory/DbObjectContext + `SqlRepositoryBase<T>`** (§3) + os **auto-registos** (§2/§3). **Duas dependências a resolver:** (a) `OB.DynamicLinqCore` (tradução de query — copiar se disponível, senão simplificar o motor de filtro/projeção); (b) `ICacheProvider`/caching (**remover** o bloco no slice mínimo — CRUD puro funciona sem cache). **Dois motores a reimplementar localmente:** conversores DTO (a decco-maker já prevê conversores Receive/List próprios — não depender do motor da base) e, se for fachada, o HTTP client. Tudo o resto da família permanece só-DLL, **fora do slice**.

## Porquê / ligação
Isto confirma que o **Decco.API moderno não deve depender dos pacotes `OB.*`** (feed corporativo): reimplementa-se o mínimo (Criteria + UoW EF Core + SqlRepositoryBase + DI por convenção). Comparar com o `OB.Api.Core` legado (`reference/08`) mostra a **evolução** do mesmo padrão UoW/repositório de EF6/Unity → EF Core/DI nativa.
