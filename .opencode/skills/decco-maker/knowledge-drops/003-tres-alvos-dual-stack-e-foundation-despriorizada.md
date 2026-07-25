# 003 — Três alvos, dois stacks (moderno + legado) e Foundation despriorizada

- **Data de registo:** 2026-07-12
- **Fonte:** decisão do utilizador (manter o padrão legado para estudo de tech ainda usada no mercado; foco em OB.API + Decco.API; minimizar Foundation)
- **Tipo:** decisão de rumo (⚠️ ajusta o escopo definido em `001`/`002` e nas `reference/`)
- **Afeta:** SKILL.md, reference/05, reference/06, reference/07, e a prioridade da Foundation.API

## A decisão
O sandbox passa a ter **TRÊS alvos**, e a skill carrega o **conhecimento profundo dos DOIS stacks reais**:

1. **Decco.API** — core **moderno** (.NET 8, EF Core + Dapper, DI nativa). Ver `reference/06` (stack moderno).
2. **Decco.Legacy.API** — **novo alvo**: réplica **FIEL do legado** OB.API (.NET Framework 4.8, EF6/EDMX, Unity+AOP, Web API 2/OWIN, ServiceStack.Redis, WCF-client), sobre o **mesmo DeccoDB**. Ver `reference/05` (dossiê legado) + `reference/07` (blueprint da réplica).
3. **Foundation.API** — fachada (papel Partners). **Despriorizada** por ora.

A skill agora é **dual-stack didática**: além de gerar/analisar o Decco, mantém dossiês profundos do **moderno** (`reference/06`) e do **legado** (`reference/05`) reais da Omnibees, para ensinar tecnologias legadas ainda praticadas no mercado.

## Porquê (obrigatório nesta skill)
- O utilizador **já domina a Partners** (fachada moderna) → a Foundation.API ensina pouco de novo agora; despriorizar **maximiza** o tempo no que é novo para ele: **entender o OB.API** e **construir o core (Decco.API)**.
- Tecnologias legadas (EF6, Unity/AOP, WCF, .NET Framework) **ainda têm uso prático** em manutenção corporativa → vale manter o conhecimento **e** ter uma réplica onde exercitá-lo.
- Ter **o mesmo core em dois stacks** (Decco.API moderno × Decco.Legacy.API legado) transforma a diferença tecnológica no **próprio objeto de estudo** (comparação linha a linha).
- **Alternativa rejeitada:** só o moderno (perde o contato com legado que o utilizador pediu para manter); só o legado (contraria a decisão `002` de que serviço novo nasce moderno). A resposta é **os dois, comparados**.

## Como aplicar a partir de agora
- Pedido sobre **como o OB.API/legado funciona** → `reference/05`; sobre **construir a réplica legada** → `reference/07`; sobre **o moderno/Partners** → `reference/06`; sobre **o core moderno Decco.API** → `reference/02`/`06`.
- Ao gerar a **Decco.Legacy.API**: stack fiel de `reference/07` (incl. reimplementar as abstrações de `OB.Api.Core`). Ao gerar a **Decco.API**: stack moderno (`knowledge-drop 002`).
- **Foundation.API:** responder se perguntada, mas **não puxar** trabalho para lá proativamente; sugerir focar no core/legado.
- Manter o **princípio comparativo**: sempre que possível, mostrar "no moderno assim / no legado assado" (é a diretriz 6 de quadros comparativos, agora com um caso canônico real).

## Fio a puxar
Ferramental do legado: EDMX exige Visual Studio; ServiceStack.Redis 3.9.71 é EOL. Registado em `open-questions/` (Q-005) — decidir a estratégia de fidelidade vs praticidade antes de gerar a réplica.
