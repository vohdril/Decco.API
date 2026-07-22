# DECCO-PROGRESS — tracking de progresso do projeto

> Estado DECLARADO do progresso deste repositório, por track e Tier. Fica aqui, versionado no git —
> a skill `decco-maker` nunca guarda estado. Ao rodar um diagnóstico (`recipes/05`), a skill cruza este arquivo com o **código**;
> **o observado (código) vence** o declarado. Marque `[x]` ao fechar E verificar um checkpoint.

```yaml
decco_progress: v1
updated: 2026-07-22
tracks:
  back_moderno: { tier: 0, status: em-andamento }
  back_legado:  { tier: null, status: nao-iniciado }
  front:        { tier: null, status: nao-iniciado }
  auth:         { fase: null, status: nao-iniciado }
  db:           { tier: 0, status: completo }
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

**Notas do operador:**
- DB0 concluído com schema expandido do lore brasileiro
- Próximo: fechar back Tier 0 (repo-ef, repo-dapper-sp, manager, host-di, controller-fino)
