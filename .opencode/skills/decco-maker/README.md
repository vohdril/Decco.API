# Decco Maker — README

Skill **dual** (análise + geração) e **didática** para o sandbox de estudos **Decco**.

## Propósito
Ajudar o Paulo a **construir e entender**, num ambiente livre e isolado, um sandbox .NET que espelha a arquitetura Omnibees
sobre o **DeccoDB** (catálogo fictício de anomalias — visão de escala global tipo MIB/Fundação SCP). **Três alvos:**

- **Decco.API** — o **core moderno** que possui o DeccoDB (papel do **OB.API**/`bhi-ob-api`, "se nascesse hoje"): envelope Request/Response, **.NET 8 + EF Core + Dapper** (stored procedures).
- **Decco.Legacy.API** — o **mesmo core, fiel ao legado** (papel do OB.API real): **.NET Framework 4.8, EF6/EDMX, Unity+AOP, Web API 2/OWIN, ServiceStack.Redis, WCF-client**. Existe para estudar tecnologias legadas ainda usadas no mercado; a comparação moderno×legado é o exercício.
- **Foundation.API** — a **fachada** em camadas (papel do **Conector Partners**): DTO V2, validação, conversores Receive/List, IdToCode/CodeToId, FusionCache, consumo por HTTP. **Despriorizada** por ora (o Paulo já domina Partners).

## Como difere das skills `partners-*`
As `partners-compass` (consulta) e `partners-maker` (geração) são **fechadas e autossuficientes** — por design, não instigam
pesquisa. A `decco-maker` **inverte** essa postura: é **didática por princípio** (explica o *porquê*, não só o *como*), tem
escopo tecnológico **mais amplo**, carrega os **dois stacks reais** (moderno + legado) dissecados, **instiga pesquisa** (fecha
com um fio a puxar; regista dúvidas em `open-questions/`) e propõe **trilhas de aprendizagem** em camadas (tiers).

## Estrutura
- `SKILL.md` — roteador + princípios (didático/pesquisa) + regras de ouro (incl. foco em tiers) + diretrizes + meta-regra de enriquecimento.
- `reference/` — a fotografia factual, destilada da leitura dos repos reais (`C:\Git\bhi-ob-api`, `partners-api`, `partners-dtos`), dos **fontes das DLLs** (OB.Api.Core, OB.Api.Base, OB.API, OB.API.PAR) e do `decco.sql`:
  - `01` papéis/topologia/nomes · `02` conexão com banco/config · `03` envelope+fio+camadas · `04` IdToCode+DeccoDB
  - `05` stack **legado** real (OB.API) · `06` stack **moderno** real (Partners)
  - `07` blueprint da **Decco.Legacy.API** · `08` fonte real do **OB.Api.Core** (infra legada) · `09` fonte real do **OB.Api.Base** (base moderna) · `10` envelope real + **molde PAR**
  - `11` **foco em tiers** (agora vs depois) + a **visão lúdica** de escala global
- `recipes/` — moldes acionáveis (`00` decisão/roteamento; `01` slice vertical de uma entidade, alinhado aos tiers).
- `templates/` — moldes de código; **solidificam** quando o primeiro slice for gerado (ver `templates/README.md`).
- `knowledge-drops/` — enriquecimento com **conhecimento resolvido** (padrões/decisões/correções). `INDEX.md` + `000-exemplo` + `001`–`005`.
- `open-questions/` — a inovação didática: **dúvidas em aberto / investigações pendentes**, com trilha de como resolver.

## Foco atual (linha não-destrutiva)
Trabalhar por **tiers** (ver `reference/11`): 🟢 **Tier 0** = 1 entidade (`Anomalia`) ponta a ponta, **molde PAR** (`reference/10`),
nos dois tracks; 🟡 **Tier 1** = IdToCode/Criteria/cache/validação; 🔵 **Tier 2** = escala (réplicas, cache distribuído, eventos,
observabilidade). Um tier por vez; o que é de tier futuro fica **documentado, não gerado**.

## Fontes
Análise (somente leitura) dos repos em `C:\Git`; fontes das DLLs em `OneDrive\...\Documentos\Omnibees\*.zip` (dissecados);
skills `partners-compass`/`partners-maker`; blueprints de estudo em `~/Downloads/Decco-blueprint-0{1,2,3,4}-*.md`; e o `decco.sql`.
