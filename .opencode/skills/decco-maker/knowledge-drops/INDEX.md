# Knowledge Drops — índice e procedimento (decco-maker)

Os **drops** registam **conhecimento resolvido** (padrão/decisão/correção) que apareceu nas interações e ainda não está nos
`reference/`/`recipes/`/`templates/`. Para **dúvidas em aberto** (o que ainda NÃO se sabe), use `open-questions/` — não os drops.

> Em conflito entre um `reference/`/`recipe/`/`template/` e um drop, **o drop mais recente vence**. A base é a fotografia;
> os drops são as atualizações. Ciclo de vida previsto: **open-question → (resolvida) → knowledge-drop → (madura) → promovida para reference/recipe**.

## Procedimento de registo (5 passos)
1. Dissecar o material novo e confirmar contra o que já está na base.
2. Escolher o tipo: **padrão novo** | **complemento** | **correção/atualização**.
3. Criar `NNN-tema-curto.md` (numeração sequencial; ver "próximo número" no fim).
4. Se for **correção**, **NÃO apagar** o texto-base — anotar no ficheiro afetado: `⚠️ Atualização (AAAA-MM-DD): ver knowledge-drops/NNN-...`.
5. Atualizar este índice (linha + "próximo número").

## Template de um drop
```markdown
# NNN — <título curto>

- **Data de registo:** AAAA-MM-DD
- **Fonte:** <mensagem do utilizador / código / experimento / decisão de design>
- **Tipo:** padrão novo | complemento | correção/atualização
- **Afeta:** <reference/0X-...> e/ou <recipes/...> e/ou <templates/...> (se aplicável)
- **Camada:** Decco.API (core) | Foundation.API (fachada) | ambos | banco

## O padrão / a mudança
<descrição factual + exemplo (antes/depois quando for correção)>

## Porquê (obrigatório nesta skill)
<a razão de desenho; a alternativa rejeitada; o que quebraria se fosse diferente>

## Como aplicar a partir de agora
<a regra acionável que a skill deve seguir nas próximas análises/gerações>

## Fio a puxar (opcional)
<uma pergunta/experimento que este padrão abre>
```

## Índice
- `000-exemplo-como-estender.md` — modelo de formato (não é conteúdo real).
- `001-principio-didatico-e-pesquisa.md` — a diretriz central: didática + instigar pesquisa (o que difere das partners-*).
- `002-decco-api-arquitetura-legada-tech-moderna.md` — decisão de rumo: Decco.API mantém a ARQUITETURA do OB.API, mas em TECNOLOGIA moderna (.NET 8/EF Core/Dapper/DI nativa), alinhada ao stack corporativo da Partners.
- `003-tres-alvos-dual-stack-e-foundation-despriorizada.md` — três alvos (Decco.API moderno + Decco.Legacy.API legado fiel + Foundation despriorizada); a skill passa a carregar os dois dossiês de stack reais (`reference/05` legado, `reference/06` moderno) + blueprint da réplica legada (`reference/07`).
- `004-sem-frontend-e-mapa-de-fontes-externas.md` — os 3 repos são backend-only (sem front-end); mapa de disponibilidade dos fontes externos: `Partners.Dtos` e o envelope `OB.BL.Contracts` (in-tree no bhi-ob-api) têm fonte no disco; `OB.Api.Core` e `OB.Api.Base.*` só existem como DLL → reimplementar.
- `005-fontes-dissecados-par-molde-e-foco-em-camadas.md` — fontes agora disponíveis (OB.Api.Core, OB.Api.Base parcial, OB.API, OB.API.PAR) dissecados (`reference/08/09/10`); **PAR é o molde** (não o monólito); e o **método de foco em 3 tiers** (`reference/11`, linha não-destrutiva) com a visão lúdica de escala global.
- `006-frontend-real-e-decisao-banco-local-tier0.md` — OB.API é API-only (front em outro repo, como o Partners; único artefato = SDK Angular gerado por nswag; sem micro-frontend à vista); **decisão de banco local do Tier 0** (SQL Server Docker/LocalDB + `decco.sql`, database-first, SPs via Dapper); e a **trilha futura de front-end** (OpenAPI+CORS já; framework a decidir em Q-006).
- `007-auth-permissionamento-nao-e-identity-e-plano-de-espelho.md` — auth/permissionamento: **NÃO é ASP.NET Identity** (nem Partners nem OB.API); token externo (IdentityServer); dois stores (core = tabelas hand-made/database-first no banco Omnibees; conector = `ApiPermission` EF code-first + `HasData`); **auth é Tier 2** (Tier 0 sem auth) e o plano de espelho no Decco. Detalhe em `reference/12`.
- `008-auth-rica-e-autossuficiencia.md` — dados sensíveis verificados (senha só hasheada/validação externa; segredos via Vault Transit; PCI); **auth RICA do Decco** (papéis + permissões + autorização a nível de recurso via **clearance-level** + **sítio**; emissor de token local; `PasswordHasher`/PBKDF2; resource-based authorization) em `reference/13` + `recipes/02`; e a decisão de **autossuficiência** (reference/recipes são a fonte; repos/zips são proveniência opcional).
- `009-database-first-vs-code-first-e-hibrido.md` — verdict: o ecossistema é **database-first orientado** (OB.API EDMX; Partners scaffold+`EnsureCreated`, **sem migrations**) → por isso evita Identity (code-first/migração). Decisão **HÍBRIDA** do Decco: **domínio database-first** (rodar `assets/decco.sql`; scaffold preserva OOP; SPs via Dapper — `reference/14`) + **auth code-first**. Skill agora **carrega `assets/decco.sql`** (autossuficiência do banco). A skill NÃO gera o schema do domínio.
- `010-persistencia-poliglota-e-change-tracking.md` — o ecossistema é **poliglota** (SQL Server + MySQL + Couchbase + Elasticsearch + Redis + Vault + Kafka); **change-tracking em escala NÃO é SQL** — é change-as-event (Kafka/ES); SQL só guarda estado atual + "última alteração" (colunas + trigger, que o `decco.sql` já tem). Mapa DB-tech→tier + estratégia de tracking em camadas em `reference/15`.
- `011-preflight-primeira-execucao-e-banco.md` — comportamento de arranque: **a skill provisiona o DeccoDB** (cria se não existir, rodando `assets/decco.sql`), faz **preflight** de pré-requisitos por tier, conduz a **primeira execução guiada** (um checkpoint por vez), usa **LocalDB por default** (Docker como 1ª etapa) com connection string **trocável num único ponto**, e trata **dúvidas fora de ordem** de forma didática. Recipe em `recipes/03`.
- `012-frontend-real-stack-e-esfera.md` — abre a **trilha de front-end** (FE tiers 0–4) e carrega uma **implementação de referência FE Tier-0 executável** em `assets/frontend-tier0/` (análogo do `decco.sql`): Vite+React 19+TS, **Radix Dialog** (modais), **TanStack Query** (server-state), **rhf+zod** (forms), design system **JARVIS** por tokens, **estados** Skeleton/Loading/Empty/Error, **authz na UI** (clearance+sítio), **sandbox de componentes** editável e **esfera** em canvas 2D (→ r3f no FE Tier 3). Resolve a parte "framework" de `Q-006`. Detalhe em `reference/16`.
- `013-frontend-junto-do-tier0-e-seam-mock-live.md` — consolida: (1) o **FE Tier-0 sai JUNTO do back Tier-0** (mesmo checkpoint — `recipes/04`), roda em **modo mock** sem back; (2) **seam de dados obrigatório** mock↔Decco.API — UI depende só do contrato `DeccoApi` (`data/gateway.ts`), impl **mock**/**HTTP** escolhidas num **ponto único** (`data/index.ts`) por env/localStorage + **seletor DADOS** em runtime; trocar **nunca toca na UI**. Pesquisa escolheu **gateway+factory** no Tier-0 (zero deps) e **MSW** como upgrade. Nova **Regra de ouro 11** no `SKILL.md`.
- `014-tracking-diagnostico-e-imutabilidade.md` — três decisões acopladas: (1) o **estado de progresso vive no PROJETO** (`DECCO-PROGRESS.md` na raiz do repo, ids de checkpoint = rubrica), nunca na skill; (2) **diagnóstico assertivo a frio** (`recipes/05` + `reference/17`) que cruza **código × rubrica de sinais por Tier**, deriva o **Tier observado por track**, detecta fora-de-sequência e **reconcilia** (o **observado vence** o declarado), sem contexto de conversa; (3) **skill IMUTÁVEL por padrão** — não se autoedita em nenhum ambiente salvo pedido explícito (meta-regra de enriquecimento vira **opt-in**; Diretriz 3 passa a propor, não executar). Novas **Regras de ouro 12 e 13**.
- `015-nao-corrigir-e-só-por-pedido.md` — duas diretrizes complementares: (A) **não apontar nem corrigir** imprecisões na skill — pequenos erros são propositais para aprendizado; (B) **só alterar** a skill ou o projeto **sob pedido explícito** do operador, nunca por iniciativa própria. Prevalece sobre diretrizes de proatividade.
- `016-database-tier-v2.md` — nova track **DB** (DB0-4) com schema expandido do lore brasileiro (Cognição, Periculosidade, OA, Laboratórios, Protocolos, Notificações); versionamento da skill para v2.
- `017-frontend-dashboard-DB2-e-sidebar-contraivel.md` — sidebar contraível com categorias, glossário visual de catálogos, modal de anomalias em 3 tabs (Radix Tabs), dashboard hero com robô Eva-style + tiers grid; skill v3.
- `018-frontend-definitivo-v5.md` — **Fotografia definitiva do Tier-0**: Dashboard completo (56 source files, RobotAnatomy decomposto, ParticleSphere, Glossario com loading/error), Decco.API .NET 8 (6 projetos, envelope, 10 controllers, DI por convenção), DeccoDB (1346 linhas, lore brasileiro, 22 tabelas, SPs, triggers). assets/frontend-tier0/ e assets/decco.sql substituídos. Skill v5.
- `019-modais-tier-hello-world.md` — Dashboard v0.0.2: cada card do roadmap abre modal com intro explicativa + código hello world real da tecnologia (FE0-4, BE0-3, DB0-4). tierContent.ts, onTierClick no TierRoadmap, Modal no DashboardPage. Skill v6.
- Dashboard v0.0.4: hero robot substituído por SVG importado, modal de usuário com 10 campos, footer, header refinado, modal size ml (640px). Solution Decco.API reorganizada em SolutionLayer/DataLayer/BusinessLayer/TestLayer. Skill v8.
- `020-modernizacao-datalayer-entity-config-scan.md` — DataLayer do Decco.API migrado do DbContext monolítico (627 linhas, 24 DbSets + OnModelCreating gigante) para **Abordagem 2**: interface `IEntity` como marcador, `Set<T>()` no lugar de DbSet props, `IEntityTypeConfiguration<T>` classes separadas descobertas via `ApplyConfigurationsFromAssembly`. DbContext reduzido a ~15 linhas. Nova entidade exige só: (1) classe model implementando `IEntity`, (2) classe de configuração. Skill v7.
- `021-getpathcenter-utility.md` — Utility `getPathCenter` para calcular centro geométrico de paths SVG a partir dos comandos do path, usada para posicionar circles de glow-preenchido nos robôs do Dashboard. Regra de raio sugerido e tabela de verificação com 6 partes do corpo.
- `022-componentes-modal-sandbox.md` — Análise do MUI All Components como referência para a modal de Componentes do sandbox Decco: categorias (Inputs, Data Display, Feedback, Navigation, Surface, Data Viz, Utils), layout com acordeão/grid de cards, mini live demos com toggle de props/cor/estado em tempo real, respeitando a identidade visual Decco (tokens, robôs, transições).
- `024-tier0-definitivo-com-login.md` — **Redefinição definitiva dos Tiers** a partir dos repositórios reais taggeados como `modelo-template`. Tier-0 = o que existe (Decco.API v0.0.1 + Decco.Dashboard v0.0.1 com micro-frontend). Tier-1 = problema integrador "implementar tela de login" que cruza DB (Docker) + BE (JWT) + FE (login real) + GT (commits). Tier-0 não se refaz. Atualiza `reference/11`, `reference/16`, `reference/17`, `reference/18`. Skill v10.

- `025-validacao-estruturas-partners-omnibees.md` — auditoria cruzada dos 3 repositórios contra as referências Partners e Omnibees: stacks moderno/legado/banco/lore brasileiro estão **corretos**; `tierContent.ts` BE0 tem discrepâncias (7 projetos vs 4 descritos, envelope com `Status` enum vs `bool Success`); features BE1+ são planejadas, não existentes. Documenta decisões de alinhamento.

- `026-pipeline-execucao-e-didatica.md` — **Pipeline de execução tier-aware** (skill v11): validação em background a cada invocação (git status/log/tag + diff × tabela tecnologia→tier × rubrica), trio **anterior/atual/próximo** por track, conflito de tier como alerta pré-geração, progresso assimétrico como **bússola de estudo**, e `DECCO-BACKLOG.md` no projeto como canal de achados (tecnologias novas × roadmap + protocolo `[FEEDBACK]`). **Modelo didático de respostas** (7 seções, molde = resposta Docker desta sessão): "Problemas que resolve" enriquecida com pesquisa de mercado + docs oficiais citadas; conceitos com mínimo 5 linhas; guia em dois caminhos (CLI+GUI); exemplo operável; roteiro de investigação; gancho ao próximo passo. Novas Regras de ouro 15/16, Diretriz 11, `reference/20`.

> Próximo número livre: **027**.
