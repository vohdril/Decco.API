# 014 — Tracking de progresso, diagnóstico assertivo e imutabilidade da skill

- **Data de registo:** 2026-07-20
- **Fonte:** pedido do Paulo — vai implementar o Decco noutra máquina/ambiente, **independente do Claude** (teste real de autossuficiência da skill). Quer: (1) manter um **tracking do Tier** do projeto num repositório; (2) que a skill faça **diagnósticos** cruzando o código com os conceitos esperados por Tier (workflow não-destrutivo); (3) que a skill **não se atualize/autocorrija** em ambientes diferentes, salvo pedido explícito; (4) diagnóstico **assertivo a partir de conversa sem contexto**.
- **Tipo:** padrão novo (introduz tracking + diagnóstico) + política (imutabilidade)
- **Afeta:** `SKILL.md` (Regras de ouro **12** e **13**; Diretriz 3; meta-regra de enriquecimento agora **opt-in**), `reference/17` (novo), `recipes/05` (novo), `templates/DECCO-PROGRESS.md` (novo)
- **Camada:** meta (comportamento da skill) + todos os tracks

## O padrão / a mudança
Três decisões acopladas:

**(1) O estado de progresso vive no PROJETO, não na skill.** Cada repositório Decco carrega um `DECCO-PROGRESS.md` na raiz
(versionado): Tier declarado por track (Back moderno / Back legado / Front / Auth / Persistência) + checklist de checkpoints com
os **mesmos ids** da rubrica. Template em `templates/DECCO-PROGRESS.md`. A skill **nunca** guarda estado de projeto.

**(2) Diagnóstico assertivo a frio** (`recipes/05` + `reference/17`): a comandos como *"rode um diagnóstico do progresso atual"*
ou *"analise X e Y"*, a skill descobre os tracks pelos artefatos, **checa sinais detectáveis** de cada conceito no código (rubrica
`reference/17` §2 — pacotes, arquivos, símbolos, objetos SQL), deriva o **Tier observado por track** (maior tier com todos os
obrigatórios; senão *"Tier N em andamento (x/y)"*), detecta **fora-de-sequência** (conceito de tier superior com o corrente aberto)
e **reconcilia** com o `DECCO-PROGRESS.md` — onde **o observado (código) vence** o declarado. Tudo **sem depender de contexto de
conversa**. Relatório lidera pelo tier observado, lista faltantes com ação, e fecha com um fio a puxar.

**(3) Skill IMUTÁVEL por padrão** (Regra de ouro 12): a skill **não cria nem edita os próprios arquivos** (reference/recipes/
templates/knowledge-drops/open-questions/assets) em **nenhum** ambiente — só sob pedido **explícito**. A meta-regra de
"enriquecimento contínuo" virou **opt-in**; a Diretriz 3 passou a **propor** (não executar) registros. O diagnóstico é **read-only**:
só escreve no `DECCO-PROGRESS.md` (do projeto) se o operador pedir.

## Porquê (obrigatório nesta skill)
- **Imutabilidade = mesma referência em toda máquina.** O objetivo do Paulo é estudar em ambientes diferentes com a **mesma** skill
  estável. Se a skill se autoeditasse (drops/open-questions) em cada ambiente, divergiria — e reintroduziria o risco de escrita
  acidental (como o vazamento de anotações no asset que corrigimos). Separar **skill (referência imutável)** de **projeto (estado
  mutável)** é o mesmo *dependency inversion* do seam de dados: o que muda fica fora do que é estável.
- **Sinais detectáveis > declaração.** Um diagnóstico que confiasse só no `DECCO-PROGRESS.md` mentiria quando o arquivo ficasse
  desatualizado. Cruzar com o **código** (o observado vence) torna o diagnóstico confiável a frio — que é exatamente o teste de
  autossuficiência pedido. A rubrica foi extraída dos dossiês `reference/05–16` para cada conceito ter um sinal concreto.
- **Não-bloqueante e por-track.** Tiers são **guia, não cerca** (Diretrizes 9–10): o diagnóstico **situa**, nunca reprova; e cada
  track tem tier próprio (nunca somar/mediar) — reflete que Back/Front avançam em ritmos diferentes.
- **Alternativa rejeitada:** guardar o progresso dentro da skill (ex.: um `state.json` em `decco-maker/`) — quebraria a
  imutabilidade e não seria versionado com o código do projeto. Rejeitada.

## Como aplicar a partir de agora
- **Nunca** editar arquivos da skill sem pedido explícito. Ao ver algo digno de registro, **sugerir** e esperar o "pode registrar".
- Diagnóstico → `recipes/05`; a decisão de tier → `reference/17`. Sempre reportar **observado por track**, faltantes **com ação**,
  avisos **não-bloqueantes**; escrever no `DECCO-PROGRESS.md` só sob pedido.
- Ao criar/scaffoldar um projeto, incluir o `DECCO-PROGRESS.md` (de `templates/`) na raiz do repo e marcar o Tier inicial.
- Manter os **ids** de checkpoint idênticos entre `reference/17`, `templates/DECCO-PROGRESS.md` e o relatório (mapeamento 1:1).

## Fio a puxar
Rodar o diagnóstico **antes e depois** de fechar um checkpoint e comparar o `x/y` — o feedback-loop que torna o workflow
não-destrutivo mensurável. E, quando o back Tier-0 nascer noutra máquina, verificar se o diagnóstico o detecta corretamente **sem
nenhum contexto** — é o teste final da autossuficiência da skill.
