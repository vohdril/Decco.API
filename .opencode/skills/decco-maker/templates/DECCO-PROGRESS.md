# DECCO-PROGRESS — tracking de progresso do projeto

> **O que é:** o estado DECLARADO do progresso deste repositório Decco, por **track** e **Tier**. Fica **aqui, no projeto**
> (versionado no git) — a skill `decco-maker` **nunca** guarda estado.
> **Como a skill usa:** ao rodar um diagnóstico (`recipes/05`), ela cruza este arquivo (declarado) com o **código** (observado) e
> **o observado vence** — divergências viram avisos, nunca erros.
> **Como atualizar:** marque `[x]` ao **fechar E verificar** um checkpoint (ou peça: *"atualize o progresso"*). Ajuste também o
> bloco `yaml` de resumo e a data. Os `[ ]` que sobram são o mapa do que falta, na ordem sugerida (workflow não-destrutivo).
> **Ids** dos itens = conceitos-checkpoint da rubrica `reference/17` §2 (o diagnóstico usa os mesmos nomes).

```yaml
decco_progress: v1
updated: AAAA-MM-DD
# tier: número do Tier em foco | status: nao-iniciado | em-andamento | completo
tracks:
  back_moderno: { tier: null, status: nao-iniciado }   # Tier 0|1|2
  back_legado:  { tier: null, status: nao-iniciado }   # Tier 0|1|2
  front:        { tier: null, status: nao-iniciado }   # FE 0..4
  auth:         { fase: null, status: nao-iniciado }   # opt-in: 2.0|2.1|2.2
  db:           { tier: null, status: nao-iniciado }   # DB0..4
```

## Track A — Back moderno (`Decco.API`, .NET 8)
### 🟢 Tier 0 — slice vertical · checkpoint: *criar/listar/buscar Anomalia pelo envelope*
- [ ] `envelope` — `Decco.Contracts` (RequestBase/ResponseBase/Status/Error)
- [ ] `dbcontext-efcore` — 1 DbContext EF Core 8 + 1 DomainScope
- [ ] `repo-ef` — CRUD por EF/LINQ
- [ ] `repo-dapper-sp` — `sp_Anomalia_Buscar` via Dapper
- [ ] `manager` — manager de domínio (molde PAR)
- [ ] `host-di` — host ASP.NET Core + DI nativa
- [ ] `controller-fino` — controller V2 que só delega
### 🟡 Tier 1 — valor de domínio · checkpoint: *listo filtrado/paginado, com códigos e cache dos catálogos*
- [ ] `idtocode` · [ ] `conversores` · [ ] `cache-local` · [ ] `criteria` · [ ] `fluentvalidation` · [ ] `fachada-foundation` *(opcional)*
### 🔵 Tier 2 — escala (um item = um checkpoint)
- [ ] `replicas-ro` · [ ] `cache-distribuido` · [ ] `observabilidade` · [ ] `eventos-kafka` · [ ] `multi-scope` · [ ] `auth-rica` *(opt-in)*

## Track B — Back legado (`Decco.Legacy.API`, .NET Framework 4.8)
### 🟢 Tier 0 — mesmo core, stack legado
- [ ] `envelope` (Decco.Legacy.Contracts) · [ ] `dbcontext-ef6` (EDMX) · [ ] `repo-ef6` · [ ] `repo-dapper-sp` · [ ] `manager` · [ ] `host-owin` (Unity/OWIN/WebApi2) · [ ] `controller-fino`
### 🟡 Tier 1 — [ ] cache legado (`ServiceStack.Redis` via `ICacheProvider`)
### 🔵 Tier 2 — [ ] `owin-oauth` (CustomPrincipal) · [ ] `wcf-client` (client-only) · [ ] escala (RoundRobin)

## Track C — Front (React/Vite)
### 🟢 FE0 — console executável em modo mock · checkpoint: *login, filtro por clearance, modal Radix, 4 estados*
- [ ] `seam-dados` — contrato DeccoApi + factory + toggle mock/live
- [ ] `login-tipos` — login + rotas protegidas
- [ ] `dashboard` — dashboard + esfera
- [ ] `anomalias-page` — página de Anomalias sobre mocks
- [ ] `estados-4` — Skeleton/Loading/Empty/Error (cenários)
- [ ] `modais-radix` — modais acessíveis (Radix)
- [ ] `authz-ui` — autorização na UI (clearance+sítio)
### 🔵 FE1 — [ ] flip do seam p/ `live` (httpApi + VITE_DATA_SOURCE) · [ ] SDK NSwag · [ ] MSW
### 🔵 FE2 — [ ] data-grid (TanStack Table) · [ ] gráficos (Recharts/visx) · [ ] toasts em escala (sonner)
### 🟣 FE3 — [ ] esfera com shaders (react-three-fiber)
### ⚫ FE4 — [ ] Storybook · [ ] mobile · [ ] micro-frontends

## Vertical Auth (Tier 2, opt-in — depois do Tier 0 do back)
- [ ] `auth-token` (2.0) — JWT + `POST /api/Auth/Token` + PasswordHasher/PBKDF2 (sem ASP.NET Identity)
- [ ] `auth-apipermission` (2.1) — `ApiPermission` + `HasData` + deny-by-default
- [ ] `auth-clearance-sitio` (2.2) — clearance+sítio (filtro-de-query **e** check por-item) + `IDataProtector` + boundary PCI

## Track D — Database (DB0-4 — modelagem de dados)
### 🟢 DB0 — SQL + Lore Brasileiro · checkpoint: *schema com Cat_CognicaoAparente, Cat_Periculosidade, Laboratorio, ProtocoloContencao, NotificacaoAnomalia*
- [ ] `cognicao-tabela` — `Cat_CognicaoAparente` com SE/SA/IN/AA
- [ ] `periculosidade-tabela` — `Cat_Periculosidade` com 9 níveis
- [ ] `laboratorio-tabela` — `Laboratorio` (entidade de configuração)
- [ ] `protocolo-tabela` — `ProtocoloContencao` + `Protocolo_AplicadoEm` (N:N)
- [ ] `notificacao-tabela` — `NotificacaoAnomalia`
- [ ] `anomalia-oa` — `Anomalia` com `CognicaoAparenteId` e `PericulosidadeId`
- [ ] `sps-novas` — SPs para Laboratorio, Protocolo, Notificacao
- [ ] `seed-lore` — Seed data com classificação OA
### 🔵 DB1 — [ ] CRUDs de catálogo + telas (endpoints + front para Cat_CognicaoAparente, Cat_Periculosidade, Laboratorio, Protocolo, Notificacao)
### 🟣 DB2 — [ ] NoSQL: perfil completo da anomalia como documento agregado (MongoDB/Couchbase)
### 🟠 DB3 — [ ] Elasticsearch: índices + CDC para busca full-text
### ⚫ DB4 — [ ] Redis (cache de catálogos) · [ ] Kafka (eventos de domínio) · [ ] Vault (segredos)

---

## Persistência poliglota (transversal — referência técnica, track DB sobrepõe)
- [ ] `sql-verdade` (P0) — SQL Server + auditoria (colunas + trigger) + `Incidente`
- [ ] `redis-fusion` (P1) — Redis + FusionCache (L1+L2+backplane)
- [ ] `kafka-es-couchbase` (P2) — `SaveChangesInterceptor` → Kafka → Elasticsearch `changehistory-*`
- [ ] `vault-otel` (P3) — Vault/`IDataProtector` + OpenTelemetry

---
**Notas do operador** (decisões, desvios conscientes, "puxei o vertical de auth para cedo", etc.):
- …
