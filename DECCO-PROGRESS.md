# DECCO-PROGRESS — tracking de progresso do projeto

> Estado DECLARADO do progresso deste repositório, por track e Tier. Fica aqui, versionado no git —
> a skill `decco-maker` nunca guarda estado. A cada invocação, a skill roda a **validação em background**
> (`reference/20` §Parte 1: git diff × rubrica × roadmap) e cruza com este arquivo; **o observado (código/diff) vence**.
> Achados de tecnologia não contemplada + refatorações `[FEEDBACK]` → `DECCO-BACKLOG.md` (não aqui).

```yaml
decco_progress: v2
updated: 2026-07-29
tracks:
  back_moderno: { tier: 0, status: em-andamento }   # anterior: Tier-0 parcial → atual: Tier-0 x/y → próximo: Tier-1 (login)
  back_legado:  { tier: null, status: nao-iniciado }
  front:        { tier: 0, status: completo }        # FE0 (modelo-template) completo — à FRENTE do back
  auth:         { fase: null, status: nao-iniciado } # próximo vertical após Tier-0 back fechar
  db:           { tier: 0, status: completo }        # DB0 completo — à FRENTE do back
  docker:       { etapa: introducao-nginx, status: em-andamento } # primeiro contato guiado (Nginx) antes do SQL
```

## Track D — Database (DB0 — schema + lore brasileiro)
### 🟢 DB0 — SQL com Cognição, Periculosidade, Laboratório, Protocolo, Notificação
- [x] `cognicao-tabela` — Cat_CognicaoAparente com SE/SA/IN/AA
- [x] `periculosidade-tabela` — Cat_Periculosidade com 9 níveis
- [x] `laboratorio-tabela` — Laboratorio
- [x] `protocolo-tabela` — ProtocoloContencao + Protocolo_AplicadoEm
- [x] `notificacao-tabela` — NotificacaoAnomalia
- [x] `anomalia-oa` — Anomalia com CognicaoAparenteId e PericulosidadeId
- [x] `sps-novas` — SPs para Laboratorio, Protocolo, Notificacao
- [x] `seed-lore` — Seed data com classificação OA

## Track A — Back moderno (Decco.API, .NET 8)
### 🟢 Tier 0 — slice vertical (em andamento)
- [x] `envelope` — Decco.Contracts (RequestBase/ResponseBase/ErrorInfo)
- [x] `dbcontext-efcore` — DeccoDbContext EF Core 8
- [ ] `repo-ef` — CRUD por EF/LINQ
- [ ] `repo-dapper-sp` — Dapper + SPs
- [ ] `manager` — manager de domínio
- [ ] `host-di` — host + DI
- [ ] `controller-fino` — controller V2

## Track C — Front (Decco.Dashboard, React/Vite) — repo irmão
### 🟢 FE0 — console mock + micro-frontend (modelo-template) — COMPLETO
- [x] `seam-dados` · [x] `login-mock` · [x] `micro-frontend` · [x] `dashboard` · [x] `estados-4` · [x] `modais-radix` · [x] `tier-content`

## Track Docker (introdução guiada — pré-DB1)
- [x] `docker-compose-nginx` — docker-compose.yml com serviço Nginx (exercício de habituação)
- [ ] `docker-sql` — SQL Server 2022 em container (DB1) — PRÓXIMO passo

**Notas do operador:**
- DB0 concluído com schema expandido do lore brasileiro
- FE0 (Decco.Dashboard) completo — o back está ATRÁS do front e do banco: foco de estudo = fechar back Tier 0
- Introdução a Docker em andamento: Nginx operável primeiro (habituação), SQL Server container na sequência
- Objetivo declarado: banco de auth SEPARADO do DeccoDB, em container Docker (investigadores/agentes do dashboard)
- Próximo: fechar back Tier 0 (repo-ef, repo-dapper-sp, manager, host-di, controller-fino) → Docker SQL → Tier-1 (login)
