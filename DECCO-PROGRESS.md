# DECCO-PROGRESS — tracking de progresso do projeto

> Estado DECLARADO do progresso deste repositório, por track e Tier. Fica aqui, versionado no git — o hub de skills
> (`Decco.Skills`) nunca guarda estado. A cada invocação, a skill `decco` roda a **validação em background**
> (`reference/20` §Parte 1: git diff × rubrica × roadmap) e a `decco-scan` cruza com este arquivo; **o observado
> (código/diff) vence**. Achados de tecnologia não contemplada + refatorações `[FEEDBACK]` → `DECCO-BACKLOG.md` do
> workspace (não aqui).
>
> ⚠️ **Progresso reiniciado em 2026-10-09**, por decisão do operador: o estado atual do projeto é o **ponto de partida
> do Tier-0**. O estado anterior — o declarado de 2026-07-29 e o observado de 2026-10-08, com o plano — está no
> `DECCO-BACKLOG.md` do workspace, entrada **"[SNAPSHOT] Estado do projeto antes do reset do progresso (2026-10-09)"**.
> É a linha de base para a `decco-scan` comparar.
>
> **Pendente:** definir os checkpoints do novo Tier-0. A rubrica (`decco-scan/reference/17` §2) ainda descreve o Tier-0
> antigo (modelo-template), que o código já cumpre. Até ela ser redefinida, um diagnóstico vai apontar esses sinais como
> "observado-não-declarado".

```yaml
decco_progress: v2
updated: 2026-10-09
baseline: 2026-10-09   # snapshot no DECCO-BACKLOG.md do workspace
tracks:
  back_moderno: { tier: 0, status: em-andamento }   # 0 checkpoints — início do Tier-0
  back_legado:  { tier: null, status: nao-iniciado }
  front:        { tier: 0, status: em-andamento }   # 0 checkpoints — início do Tier-0
  auth:         { fase: null, status: nao-iniciado }
  db:           { tier: 0, status: em-andamento }   # 0 checkpoints — início do Tier-0
  docker:       { etapa: null, status: nao-iniciado }
```

| Track | Repositório | Tier | Checkpoints |
|---|---|---|---|
| A — Back moderno | `Decco.API` | 0 | 0 — os do novo Tier-0 estão por definir |
| B — Back legado | `Decco.Legacy.API` | — | não iniciado |
| C — Front | `Decco.Dashboard` | 0 | 0 — os do novo Tier-0 estão por definir |
| D — Database | `Decco.Database` | 0 | 0 — os do novo Tier-0 estão por definir |
| Vertical Auth | — | — | não iniciado |
| Docker | — | — | não iniciado |

**Notas do operador:**
- 2026-10-09 — progresso reiniciado: o estado atual é o ponto de partida do Tier-0 (snapshot no `DECCO-BACKLOG.md`)
