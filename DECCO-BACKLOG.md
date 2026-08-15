# DECCO-BACKLOG

> Pontos de melhoria didática e refinamento de respostas da skill **decco-maker** + achados do pipeline de execução.
> Cada entrada registra um [FEEDBACK] recebido durante a interação, a resposta original e a refatoração aplicada,
> ou uma tecnologia introduzida no projeto que encaixa no roadmap mas não estava contemplada.
> Consumido externamente para versionamento da skill (ver `knowledge-drops/026` e `reference/20` §Parte 3).

## Formato

```md
## YYYY-MM-DD — Tema

### [FEEDBACK] Título curto
**O que foi pedido:** ...
**Resposta original:** ...
**Refatoração:** ...
**Origem:** conversa/sessão <id>
```

---

## 2026-07-29 — Introdução ao Docker

### [FEEDBACK] Refatorar explicação inicial de Docker para nível iniciante absoluto + incluir Docker Desktop + abordagem de solução

**O que foi pedido:** A primeira explicação sobre Docker (container Nginx via docker-compose) deveria:
- Partir da premissa de ZERO conhecimento da pessoa
- Explicar o que é Docker Desktop e como usá-lo
- Validar os conceitos contra a documentação oficial (https://docs.docker.com/get-started/)
- Manter a estrutura passo a passo com comandos, mas com didática mais granular
- **Mínimo 5 linhas** para explicações principais
- Abordagem de **solução**: o que cada tecnologia resolve no desenvolvimento de software
- Docker Desktop: guia mais aprofundado do que encontrar, uso corporativo, **dois caminhos** (GUI + CLI/IDE)
- Final com comandos de **investigação prática** da plataforma + gancho para o próximo passo (SQL Server)
- **Seção "Problemas que [tecnologia] resolve" elaborada com riqueza** — solução enquanto tecnologia E enquanto produto no mercado de trabalho (que operações demandam, por quê), com **pesquisa em documentações oficiais** (como feito com Docker)

**Resposta original:** Explicação técnica direta com 4 conceitos (imagem/container/port-mapping/volume), docker-compose.yml de 5 linhas e lista de comandos. Pressupunha que o operador já entendia o ecossistema Docker. Não abordava "por que isso importa" nem o Docker Desktop em profundidade.

**Refatoração aplicada (arquivada nesta entrada):**
1. Sessão "O ecossistema Docker" — apresenta Docker Engine, Docker Desktop e Docker Compose como três camadas separadas, com o papel de cada uma
2. Sessão "Problemas que o Docker resolve" — contextualiza antes dos comandos, com o problema histórico + alternativa rejeitada + visão de mercado (CI/CD, microservices, cloud portability, ambientes reproduzíveis de time) ancorada em docs.docker.com + docker.com (fontes citadas)
3. Sessão "Docker Desktop" — instrução visual (whale na bandeja, dashboard) + guia das abas Containers/Images/Volumes/Extensions com ações em tabela GUI × CLI e páginas-chave da doc oficial (docs.docker.com/desktop/use-desktop/container/, /images/, /volumes/)
4. Conceitos revisitados com analogia e "por que isso importa pro software" (imagem→consistência, container→isolamento, volume→persistência)
5. docker-compose.yml explicado linha por linha com linguagem coloquial
6. Comandos em tabela comparativa GUI vs CLI, com descrição do que acontece "por baixo dos panos"
7. Final com comandos de investigação (compose top/exec/inspect, image history/inspect) + gancho para o próximo passo (SQL Server container)

**Origem:** Sessão Decco.API v0.0.1 (2026-07-29), respostas ao comando de iniciar Docker.

---

## 2026-07-29 — Versionamento da skill (v10 → v11)

### [VERSIONAMENTO] Pipeline de execução tier-aware + modelo didático + protocolo [FEEDBACK]

**O que foi pedido:** (1) apanhado completo do contexto da conversa; (2) comparação com a skill atual; (3) novo versionamento com pipeline de execução: noção de Tier anterior/atual/próximo, detecção de conflitos com o tier corrente, **validação em background a cada invocação** (git diff × RoadMaps × tecnologias esperadas) para quantificar o estado real do desenvolvimento, progresso assimétrico entre tracks como bússola de estudo, e **backlog alimentado por tecnologias introduzidas que encaixam no roadmap mas não estavam contempladas**; (4) nível de abstração de explicações = modelo da resposta refatorada de Docker, com atenção especial à seção "Problemas que resolve" (tecnologia + mercado, com varredura de pesquisas/docs oficiais); (5) versionar e commitar projetos + skill + agentes globais.

**Implementação (skill v11):**
- `knowledge-drops/026` — pipeline de execução em 4 tempos (T1 validação background → T2 quantificação → T3 orientação → T4 execução) + modelo didático de 7 seções + protocolo [FEEDBACK]/DECCO-BACKLOG.md
- `reference/20-pipeline-execucao-e-didatica.md` — especificação completa do pipeline (comandos git exatos, tabela de achados do diff, trio por track, tabela de assimetria→recomendação, modelo didático detalhado, protocolo de backlog)
- `SKILL.md` v11 — novas Regras de ouro 15 (pipeline/validação background) e 16 (modelo didático), Diretriz 11 (nível de abstração), descrição atualizada
- `templates/DECCO-PROGRESS.md` v2 — trio anterior/atual/próximo + menção ao DECCO-BACKLOG.md
- `knowledge-drops/INDEX.md` — drop 026 registrado, próximo número livre 027
- Sincronização da cópia local da skill em `.opencode/skills/decco-maker/` (projeto Decco.API)

**Achados do pipeline registrados nesta rodada:**
| Achado | Tipo | Ação |
|---|---|---|
| `docker-compose.yml` (Nginx) introduzido no projeto | tecnologia esperada do roadmap (DB1 "Docker") chegando por rota de habituação didática | checkpoint `docker-compose-nginx` no DECCO-PROGRESS.md (novo track Docker) |
| `DECCO-BACKLOG.md` criado na raiz | artefato novo do pipeline (não contemplado no roadmap GT) | incorporado à skill como protocolo (Regra 16, reference/20 §Parte 3) |
| Front (FE0) e DB (DB0) completos; back (Tier 0) em x/y | progresso assimétrico | bússola: foco de estudo = fechar back Tier 0 |
| Banco de auth SEPARADO do DeccoDB em container | decisão de arquitetura do operador (não estava explícita na skill) | registrada; referencia DB1 (auth tables) com database separado |

**Origem:** Sessão Decco.API v0.0.1 (2026-07-29), pedido explícito de versionamento.
