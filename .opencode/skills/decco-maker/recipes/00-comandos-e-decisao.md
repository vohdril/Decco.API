# recipes/00 — Comandos e decisão (por onde começar / o que gerar)

## Gatilho → ação

| O utilizador diz… | Ação |
|---|---|
| "monta/gera **os projetos** Decco/Foundation" | Criar a estrutura de soluções conforme `reference/01` (nomes canônicos). Confirmar a decisão "envelope: projeto vs pacote" antes. |
| "monta **o core** da Decco.API" | Camada core **moderna**: `DeccoDbContext` (EF Core) + repositório híbrido (EF+Dapper) + managers + controllers-envelope. **Molde = PAR** (`reference/10`). Ver `reference/02` + `reference/03` + `reference/09`. |
| "monta o core **legado** (Decco.Legacy.API)" | Réplica fiel: .NET Fx 4.8, EF6/EDMX, Unity+AOP, OWIN/Web API 2. Ver `reference/07` (blueprint) + `reference/08` (fonte do OB.Api.Core a reimplementar) + `reference/05`. Antes, checar `open-questions/Q-005` (ferramental EDMX). |
| "**conecta ao DeccoDB**" / "como chamo as procedures?" | `reference/02` (EF Core + Dapper `CommandType.StoredProcedure`). |
| "monta **o CRUD/slice** da {Entidade}" (ex.: Anomalia) | `recipes/01-slice-vertical-anomalia.md` (ponta a ponta, ordem de dependência). |
| "gera só o **conversor / validador / serviço / controller / repositório**" | O artefato pedido, seguindo `reference/03`; marcar lógica específica com `// >>> … (investigar: …)`. |
| "**explica** {tecnologia/padrão}" | Modo didático (diretriz 2): conceito + como o Decco aplica + porquê + um fio a puxar. |
| "não sei / o que estudar primeiro?" | Propor a **trilha** de `recipes/01` e perguntar em que etapa está. |

## Foco por defeito (linha não-destrutiva — ver `reference/11` + `knowledge-drops/005`)
Gerar/planear sempre no **🟢 Tier 0** (1 entidade `Anomalia` ponta a ponta, molde PAR), salvo pedido explícito de subir de tier;
nunca o projeto inteiro de uma vez. O que é de **Tier 1/2** (IdToCode, Criteria, cache, réplicas, eventos, observabilidade) fica
**documentado, não gerado** — marcar o gancho com `// >>> (escala, Tier 2: ver reference/NN)`. Track por defeito: **moderno**
(Decco.API); o **legado** (Decco.Legacy.API) é o par de comparação.

## O que INFERIR por convenção (não perguntar)
- Nomes de projeto/namespace/classe a partir do nome da entidade (`Anomalia` → `AnomaliaController`, `AnomaliaService`, `IAnomaliaCacheRepository`, `AnomaliaManager`…).
- Envelope, paginação 0-based, `Error.Code` string, DI por convenção, repositório híbrido no core.
- Endpoints do core: `POST /api/{Plural}/{Op}`.

## O que PERGUNTAR (uma pergunta objetiva, quando essencial e não inferível)
- Os **campos** da `{Entidade}Configuration` (nome + tipo), se ainda não fornecidos → *"Quais os campos da AnomaliaConfiguration? Se preferir, cole/aponte a tabela do decco.sql."*
- Decisões de rumo com trade-off real (ex.: database-first scaffold **vs** code-first migrations). Oferecer o quadro comparativo e uma recomendação, não um questionário.

## Sempre declarar assunções
Tudo que for assumido por convenção → dizer numa nota curta ("assumi X porque Y; se preferir Z, ajusto").

## Fecho obrigatório (diretriz 3)
Toda resposta substantiva termina com **um fio a puxar** (pergunta/experimento/comparação/fonte) e, se houver zona não coberta,
o oferecimento de registar em `open-questions/`.
