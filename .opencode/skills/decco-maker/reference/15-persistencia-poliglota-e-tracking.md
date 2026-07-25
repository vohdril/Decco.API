# 15 — Persistência poliglota + tracking de alterações (mapa por tier)

> Responde a duas perguntas: (a) o ecossistema usa **mais de uma tecnologia de banco**? (sim, ~7) e a quais **tiers** cada uma
> pertence no Decco; (b) como se guarda **tracking de mudanças** num banco database-first que **cresce em escala** — e se "é no SQL".
> Verificado nos repos + fontes. Auto-suficiente. **Nunca segredo em texto (o `OmnibeesHistory/App.Config` real tem creds em claro — anti-padrão a NÃO copiar).**

## 1. Persistência poliglota — o que a Omnibees realmente usa
| Tecnologia | Papel | Onde (exemplo) |
|---|---|---|
| **SQL Server** (vários catálogos) | **OLTP / fonte da verdade** — `Omnibees` (10 EDMX por domínio), `Reservations`, `OmnibeesHistory`, `OmnibeesJobs`, `LoggingFramework`, `OmnibeesEvents`, `PortalOperadoras`; + `PartnersApiDb` (EF Core) | `Web.config` connStrings; `PartnersApiDbContext` |
| **MySQL** ⚠️ | **RateShopper** (preços da concorrência) — 2º motor relacional, provider `MySql.Data` | `Web.config` `SRMContext` |
| **Couchbase** (NoSQL doc) | cache de agregados de leitura pesada + log/notification por entidade + tokens efémeros (`OBApi::User::v{n}::{id}`) | `RepositoryFactory_Couchbase.cs` |
| **Elasticsearch 8** | **busca de change-history** (índices `changehistory-myhotel`/`-pms`: rate/room, inventory, yield, PMS) | `ElasticsearchIndexes.cs`, `RepositoryFactory_Elasticsearch.cs` |
| **Redis** | cache **L2 + backplane** do FusionCache + rate-limit (moderno); L2 (legado ServiceStack) | `Program.cs`, `FusionCacheServiceExtensions.cs` |
| **Vault** | **segredos** + cifra PCI de cartão (não é store de negócio) | `Vault.cs`; `DataSecurityApiRepository.cs` |
| **Kafka** | **event log** append-only + integração assíncrona (Avro/Schema Registry) | `LogProducer.cs`, `UpdateAvailabilityProducer.cs` |

> **É poliglota por necessidade de escala:** SQL para a verdade transacional; Redis para aliviar leitura; Kafka para desacoplar; Elasticsearch para busca/change-history; Couchbase para agregados/documentos; Vault para segredos. Cada um resolve um gargalo diferente.

## 2. Tracking de alterações — os mecanismos reais (ranqueados) e o veredito
| # | Mecanismo | Onde vive | O que faz | Omnibees usa? |
|---|---|---|---|---|
| A | **Colunas de auditoria** (`DataCriacao/DataAtualizacao/Usuario*`) | SQL (colunas), preenchidas pela **app** | "última criação/modificação" — sem histórico | Sim, mas **manual/inconsistente** |
| B | **Triggers de auditoria** | SQL | — | **NÃO** (zero nos repos reais; o `TR_Anomalia_Update_Date` é só do `decco.sql`) |
| C | **Catálogo de histórico** (`OmnibeesHistory`) | SQL separado | **arquivamento/offload por idade** (dados > ~4 meses), não audit-trail; escrita por ETL externo | Sim (só 2 domínios de alto volume) |
| D | **Diff-hook de aplicação** (`OnSaving/OnSavedChangesAsync` — snapshot antes+depois via `ChangeTracker`) | App (EF Core, `OB.Api.Base.DataLayer.EF`) | gancho pronto p/ auditoria campo-a-campo | Infra **pronta**, sem consumidor visível |
| E | **Temporal Tables / CDC** (SQL nativo) | SQL | versionamento nativo de linha | **NÃO** (deliberadamente evitado) |
| F | **Change-as-event/log** — **Kafka** (moderno) + **Elasticsearch/Couchbase** (legado) | **Infra** | mudança sai do caminho transacional como **evento/log assíncrono**, pesquisável, retido à parte | **SIM — é o padrão vencedor** |

**Veredito:** "isso é feito no SQL?" — **só parcialmente**. No SQL ficam as **colunas de auditoria** (A) e o **offload por idade** (C). Mas a **espinha de escala NÃO é SQL**: a Omnibees **evita** triggers/CDC/temporal (B/E) e faz **change-as-event** (F) — o que **quem/o quê/quando** vira um **evento/log assíncrono** (Kafka), muitas vezes indexado para busca (Elasticsearch). Isso tira a auditoria do caminho quente do banco (fire-and-forget: auditar nunca derruba a operação).

## 3. Recomendação para o Decco (database-first que escala) — em CAMADAS
Não é "escolher uma"; é uma **pilha por tier**:
1. **Agora (SQL, Tier 0):** as **colunas de auditoria** (o `decco.sql` já tem `DataCriacao/DataAtualizacao/UsuarioCriacao/UsuarioAtualizacao` na `Anomalia` + o trigger `TR_Anomalia_Update_Date`) e a tabela `Incidente` (histórico de eventos). Baseline pronta. *(Como estudo, dá para experimentar UMA Temporal Table — mas saiba que a Omnibees não a usa como espinha.)*
2. **App (Tier 1/2):** um **`SaveChangesInterceptor` do EF Core** (equivalente idiomático ao diff-hook D) que calcula o diff antes/depois — o "quem mudou o quê" campo-a-campo. É onde a OOP entra no tracking.
3. **Escala (Tier 2):** publicar esse diff como **evento Kafka** (padrão `UpdateAvailabilityEvent`/`LogProducer`, fire-and-forget) e, se quiser consulta rica, **indexar em Elasticsearch** (`changehistory-*`). Isto é o alvo de escala — tira o audit do SQL.
4. **Retenção (ortogonal, Tier 2/3):** **offload por idade** para um catálogo/índice separado (padrão `OmnibeesHistory`), mantendo o DeccoDB transacional enxuto.

> Regra de ouro do tracking: **o SQL guarda o estado atual + "última alteração"; o histórico completo e a escala vivem como evento/log fora do caminho transacional.** O trigger do `decco.sql` é um ótimo ponto de partida didático — e um gancho perfeito para a lição "por que, ao escalar, migramos disto para change-as-event".

## 4. Mapa DB-tech → tier no Decco
- **Tier 0 — SQL Server (DeccoDB):** fonte da verdade (EF Core + Dapper/SP). Tracking = colunas de auditoria + trigger (já no `decco.sql`).
- **Tier 1 — Redis + FusionCache:** cache L1+L2+backplane + rate-limit.
- **Tier 2 — Kafka + Elasticsearch + Couchbase:** eventos/change-log (Kafka), busca/change-history (ES), agregados/documento (Couchbase). É aqui que o **tracking-as-event** e a busca de histórico entram.
- **Tier 3 — Vault + observabilidade:** segredos (tira senha/creds do `appsettings`) + OTel/Grafana.
- *(Poliglota-relacional — MySQL do RateShopper — é curiosidade; opcional como estudo "por que um 2º motor relacional".)*
- **Regra:** um tier = uma tecnologia = um conceito; Tier 0 é sempre a verdade, o resto são aceleradores/derivados.

## Porquê / fio a puxar
**Porquê change-as-event e não trigger:** trigger acopla a auditoria ao banco (o gargalo), incha o storage transacional e não
escala horizontalmente; evento é assíncrono, pesquisável e retido à parte. 🔎 **Fio:** no Decco, ligue o trigger do `decco.sql`
(SQL) e, em paralelo, um `SaveChangesInterceptor` que emite o diff — compare o que cada um captura ("última alteração" vs
"diff campo-a-campo") e onde cada um pesa. Depois publique o diff como evento (Tier 2) e veja o SQL sair do caminho.
