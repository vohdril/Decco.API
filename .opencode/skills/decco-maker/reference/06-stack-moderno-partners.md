# 06 — Dossiê do STACK MODERNO real (Conector Partners / `partners-api`)

> Conhecimento profundo do stack **moderno** da Omnibees — o **`api-template`/boilerplate** com que a empresa constrói serviços
> **novos**. Extraído por leitura direta de `C:\Git\partners-api` + `C:\Git\partners-dtos` e das skills `partners-compass`/`partners-maker`.
> É o **par oposto** do dossiê legado (`reference/05`) e a base tecnológica da **Decco.API** (moderna).

## Fotografia
.NET **8**, ASP.NET Core, csproj **SDK-style** + `PackageReference`. 9 projetos de produção em 3 camadas (SL/BL/DL) + `Partners.Dtos` (`netstandard2.0`, POCO-only, pacote externo). É **boilerplate**: cria-se API nova clonando e renomeando.

## Versões/pacotes-chave
| Área | Pacote | Versão |
|---|---|---|
| ORM (BD local) | Microsoft.EntityFrameworkCore.SqlServer / InMemory | 8.x |
| Cache | ZiggyCreatures.FusionCache (+ Backplane.StackExchangeRedis, OpenTelemetry) | 2.5.0 |
| Cache L2 | StackExchange.Redis / Microsoft.Extensions.Caching.StackExchangeRedis | 2.x / 8.0.11 |
| Mensageria | MassTransit.Kafka + Confluent.SchemaRegistry.Serdes.Avro | 8.2.2 / 2.4.0 |
| Validação | FluentValidation | (via OB.Api.Base) |
| Auth | Microsoft.AspNetCore.Authentication.JwtBearer | 6.0.11 (⚠️ família .NET 6 em projeto net8) |
| Segredos | VaultSharp | 1.13.0.1 |
| Observabilidade | OpenTelemetry (OTLP) + Serilog.Sinks.Graylog | 1.7.0 / 2.3.0 |
| Versionamento | Asp.Versioning.Mvc(.ApiExplorer) | — |
| Health | AspNetCore.HealthChecks.SqlServer/Redis | 8.x |
| Base interna | OB.Api.Base.* (DI/HTTP/cache/datalayer/validation) | 1.0.0.195 |

## 1. Host + DI
`Program.cs` (minimal hosting): Serilog → Telemetry (`ConfigureObTracing` + SqlClient/HttpClient/MassTransit + OTLP) → `RegisterDependencies` → cache (StackExchangeRedis + `IConnectionMultiplexer` singleton + FusionCache + rate limit Redis) → JWT → HttpClient → controllers/versioning/swagger → health checks. **DI nativa + auto-registo por convenção** (`OB.Api.Base`): marcador `IService`, `RegisterValidatorsAutomatically`, `RegisterConvertersAndMappersAutomatically`, `RegisterRepositoriesAutomatically`. Fábricas em runtime: `RepositoryFactory`, `ConverterProvider` (via `HelperFactory`).

## 2. Camadas e os 6 artefatos por entidade
SL (`REST`,`Root`) · BL (`Contracts`,`Contracts.Validation`,`Services`,`Handlers`) · DL (`Common`,`Infrastructure`,`Model`).
Fluxo CRUD V2: `JWT → AuthorizationFilter → CustomRequestValidationFilter → Controller fino → Service(envelope try/3-catch/finally + 2 logs Kafka) → Repository Remake(FusionCache) ──HTTP──► OB.API → GeneralResponseTreatment`.
Ordem: **1** contratos DTO V2 → **2** validadores → **3** conversores Receive/List → **4** repositório Remake → **5** serviço → **6** controller V2.

## 3. Contratos DTO V2 (13 por entidade)
Create/Update/Get/Delete/List × Parameters/Request/Response, sobre ~11 tipos-base de `V2.Base` (`ApiRequest`, `ApiResponse[<T>]`, `PagedApiResponse<T>`, `RouteParameters`, `PagedQueryParameters`). Herança fixa: `Update*Parameters : Create*Parameters`; Create devolve código, Update/Delete vazios, Get=`ApiResponse<Foo>`, List=`PagedApiResponse<Foo>`. Campos quase sempre `string` (`[OpenApi*]` só documenta o Swagger). Modelo-raiz `Foo`(`FooCode`,`FooInternalCode`,`Configuration`) + `FooConfiguration` (miolo editável) + `FooEnums`.

## 4. Conversores Receive/List (`AConverterMapperBase<,>`)
- **Receive/** (request→core, Create/Update): "Info" com `Configuration`+`OldObject`(delta-merge)+listas de referência; sentido inverso lança `NotImplementedException`.
- **List/** (core→response, Get/List): 4 peças — `Configuration` → `Data` (injeta `IConverterProvider`, resolve códigos) → `Get/ListResponse` — compostas via `IConverterProvider`. Delta-merge de N:N via `RelatedEntity{Key,IsDeleted}` (estado desejado; ausentes → `IsDeleted=true`).

## 5. IdToCode / CodeToId
A tradução **vive no serviço** (monta dicionários `Id↔código/nome`), o conversor só **consome** (nunca I/O). Três subsistemas: (a) Id-interno↔código-de-parceiro (`EsMappingType`+`PartnerHelper.ResolveMappingCodesFromIds(mappingName)`); (b) dados de referência (`LocalizationHelper`: país→ISO, estado/cidade→nome, idioma id→ISO); (c) ponte OTA (`Amenity.OtaCodeUid→OtaCode.CodeValue→enum`). Escrita: código desconhecido → Id 0 (o core rejeita via `[Min(1)]`).

## 6. Repositórios Remake + FusionCache
`ObApiHttpRepositoryHotelScoped<T>` = cache (FusionCache L1+L2+backplane; fail-safe 1h, eager refresh 80%, soft 1s/hard 5s, **não cacheia null**, chave prefixada por white-label) + transporte HTTP. Paginação **progressiva** por índice de UIDs com validação de integridade. Variantes: hotel-scoped (CRUD+invalidação por tag) e global read-only whole-list (reference data). Consumo da OB.API: `POST /api/{Controller}/{Action}`, Newtonsoft `DefaultValueHandling.Ignore`, `RequestBase`/`ResponseBase`, paginação **0-based** (`PageIndex=page-1`), `Status==Fail`→`ApiException`.

## 7. Transversais
- **Auth em 2 camadas:** (1) autenticação JWT (IdentityServer, sem BD); (2) autorização por endpoint via `AuthorizationFilter` global + tabela **`ApiPermissions`** (SQL `PartnersApi`, EF Core) + permissões que o user possui (OB.API, cacheado). Recusa: V1→401, V2→403. **Fail-safe: nega por padrão.**
- **Token de serviço M2M:** `client_credentials` (scope `ob.api.public`, cache −5min), header `PartnerUserId`.
- **Validação:** `FluentValidator<T>` injeta error code tipado (`ErrorCodes.GetCode()`, string); `CustomRequestValidationFilter` traduz por versão (V1 `OTA_ErrorRS`, V2 `ApiResponse`+`Error{Code,Message}`).
- **BD local:** EF Core `PartnersApiDbContext` (só `ApiPermissions` — o domínio vem por HTTP); connection string via `SqlConnectionStringBuilder`+env vars+**Vault** (creds dinâmicas `database/creds/partners-api`, AppRole); `EnsureCreated` só em Development; InMemory nos testes.
- **Kafka:** MassTransit (bus in-memory + rider), producers, Avro + Schema Registry.
- **Observabilidade/deploy:** OpenTelemetry (OTLP) + Serilog→Graylog; Docker + Helm + K8s (HPA 2..10, probes, Vault sidecar).

> **Para o Decco:** a **Decco.API (moderna)** herda daqui o ORM (EF Core), a DI nativa, System.Text.Json, cache, observabilidade e o `.csproj` SDK-style — **mas** assume o papel de *core dono do banco* (não de fachada). A **Foundation.API** (fachada, despriorizada por ora) é a que mais literalmente imita este dossiê. Contraste explícito com o legado em `reference/05`.
