# 08 — Fonte REAL do `OB.Api.Core` (infra legada) — o que a Decco.Legacy.API reimplementa

> ⭐ Antes só existia como DLL; agora o fonte veio (`OneDrive\...\Omnibees\bhi-core-api.zip`). É a camada de **infra de acesso a
> dados** (UoW/contexto/sessão) do legado — .NET Framework 4.8, **EF6 6.2.0**, Unity 4.0.1. Dissecado read-only. Isto é exatamente
> o "buraco" que `reference/07` mandava reimplementar — agora com o molde real. (Ressalva: `BusinessLayerException` e o
> `BusinessRulesInterceptionBehavior` **NÃO** estão aqui — vivem na BL/AOP do `bhi-ob-api`, ver `reference/05`.)

## Superfície mínima a reimplementar (core de 1 scope)
1. **`DomainScope` + `DomainScopeType`** — define um "banco lógico" com **duas** connection strings: `ConnectionStringName` (RW) e `ReadOnlyConnectionStringName` (RO), + `Type` (`EntityFramework=1 | SqlServer=2`). No Decco basta **1 instância** (`Decco`).
2. **`IObjectContext` (+ `ObjectContextAdapter`)** — embrulha **1 `DbContext` EF6**: `.Context`, `CreateObjectSet<T>()`=`Context.Set<T>()`, `SaveChanges()`, `DomainScope`, `UnitOfWorkGuid`.
3. **`IUnitOfWork` + `UnitOfWorkBase`** — dicionários **por scope** de contextos EF e conexões SQL; `GetContext(scope)` lazy-factory; `Save()` **só escreve contextos "sujos"** (`ChangeTracker.HasChanges()`) e traduz `DbUpdateException`→`DataLayerException`; `DiscardChanges`, `HasChanges`, `Dispose` fecha tudo + evento `OnDispose`.
4. **`ISessionFactory` + `SessionFactory`** — **1 UoW por fluxo** via `AsyncLocal<IUnitOfWork>`; `GetUnitOfWork(readOnly, scopes)`; resolve o `DbContext` por **convenção de nome** (`OB.DL.Model.{Scope}.{Scope}Context` via `Activator.CreateInstance`); **padrão shell** para chamadas aninhadas (só o UoW raiz é real; os aninhados são `UnitOfWorkShell` que NÃO dispõem o raiz).
5. **`DataLayerException` + `SqlToDataLayerException`** — a exceção de dados (com `ErrorType`+`ErrorCode`) e o extension que percorre a InnerException chain, mapeia `SqlException.Number` (ex.: **547 = violação de FK**, extrai a tabela por regex) → `DataLayerException`. É o que alimenta a tradução erro→envelope no `Save`.
6. **Bifurcação RW/RO simples** — usar `ConnectionStringName` para escrita e `ReadOnlyConnectionStringName` quando `IsReadOnly`. (Sem RoundRobin — ver "escala".)

## Assinaturas-chave (do fonte real)
- `IUnitOfWork`: `Guid`, `IsReadOnly`, `Save(int? timeout)`, `Task<int[]> SaveAsync()`, `IDbTransaction BeginTransaction(DomainScope, IsolationLevel)`, `DiscardChanges()`, `HasChanges()`, `event Action<IUnitOfWork> OnDispose`.
- `ISessionFactory`: `IUnitOfWork GetUnitOfWork(bool readOnly, params DomainScope[])`, `CurrentUnitOfWork`.
- `SessionFactory` guarda `AsyncLocal<IUnitOfWork>`; `if (UnitOfWork.Value != null) return new UnitOfWorkShell(...)` senão cria o real. Cache de contexto por `contextType.FullName + "_" + uowGuid`.

## OPCIONAL / ESCALA (deixar de fora no sandbox de 1 scope — documentado, não usado agora)
- **`DatabaseDiscovery` + `RoundRobin`** ⭐ = **roteamento para réplicas de leitura**: clona o connection string forçando `ApplicationIntent=ReadOnly` + `DataSource=<server>`, balanceia com `RoundRobinList<T>` (round-robin **ponderado**, lock, `LinkedList`), retry 3× + `RemoveBadConnection` (tira réplica má; se esvazia, reverte ao default). Gated por `IsRoundRobinActive=false`. → **peça de escala global** (ver `reference/11`); reimplementar só quando estudar leitura escalada.
- **`PerUnitOfWorkLifetimeManager`** — LifetimeManager do **Unity** que amarra o objeto ao `Guid` do UoW (`key + "_" + CurrentUnitOfWork.Guid` em `AsyncLocal`). Só necessário no track **legado** (Unity); o core moderno com DI nativa não precisa.
- **`ITransactionManager`/`TransactionManager`** (TransactionScope distribuído) — opcional; `BeginTransaction` local cobre 1 banco.
- **`OB.Api.Core.Tracing`** — wrapper fino sobre OpenTelemetry (`AddOpenTelemetry<TProgram>` liga opcionalmente HttpClient/SqlClient/WCF/AspNet/Redis + OTLP). Observabilidade — **escala**, desacoplado do UoW.
- Multi-scope (dicionários por `DomainScope`, `params DomainScope[]`) → colapsar para 1; caminho `DomainScopeType.SqlServer` (ADO/Dapper cru) → fora se o core usar só EF. `SetTrace/sp_trace_*` (profiling) e `ITransaction`/`LocalDbTransaction` (mortos) → descartar.

## Porquê / ligação com o Decco
Este é o molde do **DL do Decco.Legacy.API** (EF6). No **Decco.API moderno**, o análogo é o `OB.Api.Base.DataLayer.EF` (ver `reference/09`) — mesma ideia (UoW/SessionFactory/contexto por scope), tecnologia diferente. Comparar os dois lado a lado é o exercício. Fonte extraído em `scratchpad/omni-src/bhi-core-api`.
