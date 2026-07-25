# 10 — Envelope REAL (`OB.BL.Contracts`) + `OB.API.PAR` como molde de serviço de domínio único

> Dois fontes reais dissecados (`scratchpad/omni-src/OB.API` e `.../OB.API.PAR`). O primeiro dá o **molde fiel do envelope**
> (`Decco.Contracts`/`Decco.Legacy.Contracts`); o segundo dá o **molde de estrutura de solução** — e é o achado mais útil deste
> lote: **PAR é o mesmo ADN do OB.API, mas destilado a um domínio** → o template ideal para o Decco.

## PARTE 1 — O envelope real (fiel para `Decco.Contracts`)
Hierarquia (todos `[DataContract]`, serialização DataContract + Newtonsoft no legado; no Decco moderno = System.Text.Json):
```
ContractBase (abstract, vazia)
├── DataContractBase (+ TrackingId)      ← o `where T` dos genéricos; Error/Warning herdam daqui
│     ├── Error   (ErrorType, ErrorCode:int, ErrorSubType, Description, Data:Dictionary, MethodName; ctor a partir de Exception)
│     └── Warning (WarningType, WarningCode:int, Description, Data)
├── RequestBase : IOperationContractBase  (RequestId gerado no ctor)
│     ├── SingleRequest / SingleRequest<T>  (Item, IncludeResult) : ISingleOperationContract
│     │     ├── InsertSingleRequest<T> / UpdateSingleRequest<T>  (subclasses vazias, só nome/semântica)
│     │     └── DeleteSingleRequest<T>  (Key, [Required(blockZeroValue:true)] — NÃO usa Item)
│     ├── BulkRequest / BulkRequest<T>      (Items, IncludeResults) : IBulkOperationContract
│     └── ListBaseRequest → PagedRequestBase (PageIndex, PageSize=-1 default, ReturnTotal; hooks DefaultPageSize/MaxPageSize) : IPagedRequest
└── ResponseBase : IOperationContractBase  (Errors:List<Error>, Warnings:List<Warning>, Status; Failed()/Succeed())
      ├── SingleResponse<T> (Result)
      ├── BulkResponse<T>   (Results)
      └── PagedResponseBase (TotalRecords) → ListPagedResponse (Result: ObservableCollection<ContractBase>)
```
- **`Status`** = enum 3 estados: `Success=0 | PartialSuccess=1 | Fail=2` (ctor de `ResponseBase` inicia `Fail`; `[DefaultValue(Success)]` na serialização).
- **`IOperationContractBase`** = única interface com membro (`RequestId`); `ISingleOperationContract`/`IBulkOperationContract` = marcadores vazios.
- ⚠️ **Dívida a NÃO copiar:** o `RequestBase` do monólito arrasta props `[Obsolete]` (`RequestGuid`, `LanguageUID`, `LanguageCode`, `RuleType`). O Decco deve nascer **sem** elas.

## PARTE 2 — `OB.API.PAR`: o molde certo (SIM) para o Decco.API/Legacy
**Achado central:** PAR **não referencia** `OB.BL.Contracts` (2.059 ficheiros) — carrega o **próprio envelope mínimo** `OB.PAR.Contracts` (**35 ficheiros**), com as mesmas bases mas só o que o domínio precisa, **sem** as props obsoletas, e com `PartialSucceed()` a mais. Prova que o envelope pode e deve ser **auto-contido e enxuto** — exatamente o que `Decco.Contracts`/`Decco.Legacy.Contracts` deve ser. No PAR, cada request/response concreto herda **direto** de `RequestBase`/`ResponseBase` (ex.: `UpdateRateRoomDetailsResponse : ResponseBase`), sem Single/Bulk quando não precisa.

### Comparação (por que PAR, não o monólito)
| Métrica | Monólito `OB.API` | `OB.API.PAR` |
|---|---:|---:|
| Contratos (`.cs`) | 2.059 | **35** |
| `Get*Repository` no factory | 246 | **6** |
| `DomainScope` | dezenas | **1** (`Rates`) |
| `OB.DL.Common` (`.cs`) | 588 | **69** |
| Controllers de domínio | dezenas | **2** |

### Checklist do que COPIAR de PAR para o Decco (estrutura)
1. **Solução em 5 pastas/camada:** `SL/<Dom>.Services`, `BL/<Dom>.Contracts` + `BL/OB.BL.Operations`, `DL/OB.DL.Common` + `DL/OB.DL.Model.<Dom>`, `DM/OB.Domain`, `TL/*.Test`.
2. **Envelope próprio e mínimo** (copiar de `OB.PAR.Contracts`): `ContractBase`, `RequestBase{RequestId, IsTransactional?, RuleType?}`, `ResponseBase{Errors, Warnings, Status; Failed/Succeed/PartialSucceed}`, `Status`, `Error`, `Warning`. Concretos herdam direto.
3. **`BaseController` `[Authorize]` + controllers finos** por injeção de construtor que só delegam ao manager (zero regra de negócio).
4. **`IBusinessPocoBase`/`BusinessPocoBase`** como base dos managers (`[Dependency]` p/ `Container`/`RepositoryFactory`/`SessionFactory`; `Resolve<T>`).
5. **Padrão de operação no manager:** criar `response`, propagar `RequestId`, validar cedo (`Failed()`), abrir `SessionFactory.GetUnitOfWork(DomainScopes.X)`, pegar repositórios especializados, `TransactionScope`, `Succeed()`. **É aqui que entra** o repositório híbrido EF Core+Dapper/SP (moderno) ou EF6+Dapper (legado).
6. **DI por módulos Unity, um por camada** (`BusinessLayerModule`/`DataAccessLayerModule` + `WarmUp()`) no track legado; **DI nativa** no moderno.
7. **`DomainScopes` com UM escopo por serviço** + `IRepositoryFactory` recortado só aos repositórios do domínio.
8. **Arranque OWIN/Web API 2** (`Startup`/`WebApiConfig`) — molde direto do Decco.Legacy.API.

**NÃO copiar:** o `RafaController`/`ListHelloWorld` de exemplo (ainda com `namespace OB.REST.Services.Controllers`) e as props `[Obsolete]` do monólito.

## Porquê / ligação
PAR resolve o medo do "projeto espelhado sem estudo": em vez de replicar o monólito, o Decco replica **PAR** — pequeno, completo ponta-a-ponta, cabe na cabeça. Para a **Anomalia**, PAR é o gabarito 1:1 (controller→manager→UoW→repos→response). Fontes: `scratchpad/omni-src/OB.API.PAR/Main` e `.../OB.API/Main/BL/OB.BL.Contracts`.
