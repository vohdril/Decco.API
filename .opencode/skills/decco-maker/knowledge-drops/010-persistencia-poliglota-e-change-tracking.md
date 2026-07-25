# 010 — Persistência poliglota + estratégia de change-tracking (escala)

- **Data de registo:** 2026-07-17
- **Fonte:** deep-dive read-only (tracking/auditoria/histórico + inventário multi-DB) nos repos + fontes
- **Tipo:** conhecimento resolvido + decisão de rumo (tiers de persistência + tracking)
- **Afeta:** `reference/15` (novo), `reference/11` (tiers), `reference/02`

## A — Sim, o ecossistema é POLIGLOTA (mais de uma tecnologia de banco)
SQL Server (vários catálogos: Omnibees, Reservations, OmnibeesHistory, OmnibeesJobs, LoggingFramework, OmnibeesEvents, PortalOperadoras + PartnersApiDb) · **MySQL** (RateShopper) · **Couchbase** (doc/cache/logs/tokens) · **Elasticsearch** (busca de change-history) · **Redis** (cache L2/backplane/rate-limit) · **Vault** (segredos + cifra PCI) · **Kafka** (event log Avro). Detalhe/âncoras em `reference/15`.

## B — Change-tracking: o veredito (e "é no SQL?")
- **No SQL** ficam só: **colunas de auditoria** (manual, "última alteração") e **offload por idade** (`OmnibeesHistory` = arquivamento, **não** audit-trail). **Sem triggers, sem Temporal Tables, sem CDC** nos repos reais (deliberadamente evitados).
- **App:** existe o **diff-hook** `OnSaving/OnSavedChangesAsync` (snapshot antes+depois via `ChangeTracker`) — infra pronta, sem consumidor visível.
- **Padrão VENCEDOR (escala) = change-as-event/log:** Kafka (moderno: `UpdateAvailabilityEvent` + `Log` Avro fire-and-forget) e Elasticsearch/Couchbase (legado: `changehistory-*`). A mudança sai do caminho transacional; auditar nunca derruba a operação.
- **Resposta curta ao Paulo:** *tracking em escala NÃO é primariamente SQL* — é evento/log assíncrono (Kafka/ES). O SQL guarda estado atual + "última alteração".

## C — DECISÃO para o Decco (em camadas, por tier)
1. **SQL (Tier 0):** colunas de auditoria + trigger — o `decco.sql` **já tem** (`DataCriacao/DataAtualizacao/Usuario*` + `TR_Anomalia_Update_Date`) + `Incidente`. Baseline pronta (e ótimo gancho didático).
2. **App (Tier 1/2):** `SaveChangesInterceptor` do EF Core que calcula o **diff campo-a-campo** (o "quem mudou o quê").
3. **Escala (Tier 2):** publicar o diff como **evento Kafka** + (opcional) indexar em **Elasticsearch** para busca de histórico.
4. **Retenção (Tier 2/3):** offload por idade para catálogo/índice separado (padrão `OmnibeesHistory`).

## D — Mapa DB-tech → tier
Tier 0 = **SQL Server (DeccoDB)** (verdade). Tier 1 = **Redis/FusionCache**. Tier 2 = **Kafka + Elasticsearch + Couchbase** (eventos/change-log, busca, documento). Tier 3 = **Vault** + observabilidade. (MySQL/RateShopper = curiosidade poliglota opcional.) Regra: **1 tier = 1 tecnologia = 1 conceito**; Tier 0 é sempre a origem.

## Como aplicar
- Não puxar Kafka/ES/Couchbase/Vault para o Tier 0. O tracking do Tier 0 é o que o `decco.sql` já dá (colunas + trigger).
- Ao estudar tracking "de verdade": `SaveChangesInterceptor` (diff) → publicar como evento (Tier 2). Explicar **por que** trigger/CDC não escalam (acoplam ao banco).
- Segredos de conexão **sempre** por env var/Vault — **nunca** hardcoded (o `OmnibeesHistory/App.Config` real erra nisto).

## Fio a puxar
Ligar o trigger do `decco.sql` **e** um `SaveChangesInterceptor` em paralelo e comparar o que cada um captura ("última alteração" vs "diff") — depois publicar o diff como evento e ver o SQL sair do caminho quente.
