# 20 — Pipeline de execução tier-aware + modelo didático de respostas

> Nova referência (2026-07-29, skill v11) — ver `knowledge-drops/026`. Define **como a skill opera a cada invocação**:
> validação em background (git diff × RoadMaps), trio anterior/atual/próximo por track, detecção de conflitos,
> protocolo de backlog e o **modelo didático de resposta** (a resposta refatorada de Docker desta sessão é o molde).

## Parte 1 — O pipeline de execução (4 tempos)

### T1. Validação em background (silenciosa, a cada invocação)

Rodar **antes** de responder qualquer pedido que envolva os projetos Decco. Custo: 3 comandos git. Regra: **só anunciar o
resultado se ele mudar o rumo** (progresso novo, conflito de tier, tecnologia nova, divergência declarado×observado).

```powershell
git status --short                 # o que mudou desde o último contato
git log --oneline -5               # últimos commits (padrão de mensagem, escopo)
git tag                            # tags de checkpoint (modelo-template, v0.0.x)
# se houver mudanças não commitadas:
git diff --stat HEAD               # QUAIS áreas mudaram (contratos? UI? SQL?)
```

Cruzamento dos paths alterados com a **tabela tecnologia→tier** (`reference/11` §tabela-resumo, `reference/17` §rubrica):

| Achado no diff | Leitura | Ação |
|---|---|---|
| Path de tecnologia **do tier corrente** | progresso normal | marcar checkpoint do `DECCO-PROGRESS.md` (não editar — oferecer) |
| Path de tecnologia de **tier futuro** com corrente incompleto | fora-de-sequência | aviso didático não-bloqueante (modelo `reference/17` §4) |
| Path de tecnologia **nova, não contemplada** no roadmap, mas que encaixa | achado de backlog | **registrar no `DECCO-BACKLOG.md`** (protocolo §Parte 3) e oferecer incorporar na skill (opt-in) |
| Nada relevante | sem mudança de rumo | seguir calado para T4 |

### T2. Quantificação do estado (por track, independente)

Se T1 encontrou mudança de rumo **ou** o pedido exige, derivar o **Tier observado por track** com a rubrica de
`reference/17` §2-3. **Nunca somar/mediar entre tracks** — reportar lado a lado. O **observado (código/diff) vence** o
declarado (`DECCO-PROGRESS.md`), sempre.

### T3. Orientação: anterior → atual → próximo

Para cada track detectado, manter e narrar o **trio**:

```
Track A — Back moderno
  anterior: Tier 0 (modelo-template) ✅ — checkpoints fechados: envelope, dbcontext-efcore, ...
  atual:    Tier 0 → em x/y (faltam: repo-ef, repo-dapper-sp, ...)
  próximo:  Tier 1 (login real) — docker-compose, auth-jwt, auth-endpoint, password-hasher, user-table
```

- **Conflito de escopo:** pedido que gera código de tier futuro com o corrente incompleto → alerta **antes** de gerar
  ("isto é Tier 2+; marcar com `// >>> (escala, Tier 2: ver reference/NN)`" — Regra de ouro 9).
- **Progresso assimétrico = bússola de estudo:** reportar o desnível como recomendação do que estudar/gerar a seguir.

| Situação | Recomendação do pipeline |
|---|---|
| FE completo à frente de BE | fechar BE antes de subir ambos para o próximo tier |
| DB completo, BE/FE parciais | consumir o schema (endpoints/telas), não modelar mais |
| Track emergente à frente (ex.: MF) | reportar como fora-de-sequência legítimo; citar o que falta do tier real |

### T4. Execução do pedido

Responder no **modelo didático** (Parte 2). Ao fim, se T1-T3 encontraram achados de backlog, propor o registro
(protocolo §Parte 3). Nunca escrever na skill sem pedido explícito (Regra de ouro 12).

## Parte 2 — Modelo didático de respostas (referência de abstração)

> A resposta refatorada de Docker desta sessão ("Aqui está a versão refatorada com o nível de detalhe que você pediu:") é o
> **molde**. Toda explicação de tecnologia nova deve seguir estas 7 seções, nesta ordem.

### 1. O ecossistema X (camadas)
As peças da tecnologia e o papel de cada uma (ex.: Engine/Desktop/Compose), antes de qualquer comando. Pelo menos 5 linhas
por peça principal; cada peça ligada ao problema que resolve no desenvolvimento.

### 2. Problemas que [tecnologia] resolve (antes de qualquer comando)
A seção mais rica — **nunca resumir**. Três blocos obrigatórios:
1. **O problema histórico/prático** que existia antes da tecnologia (dor concreta, com exemplo).
2. **A alternativa rejeitada** e o que quebraria sem ela.
3. **A visão de mercado/produto:** que tipos de solução e operação demandam a tecnologia no mundo corporativo e por quê
   (ex.: Docker → CI/CD, microservices, cloud portability, ambientes reproduzíveis de time; SQL Server → OLTP de missão
   crítica, BI/analytics; JWT → SSO, APIs stateless, auth entre serviços). **Exige pesquisa** nas documentações oficiais
   (docs.docker.com, learn.microsoft.com, redis.io, ...) e no posicionamento do produto — **citar as fontes** (URL + o que
   foi extraído). Como o operador pediu: "quais tipos de soluções e operações demandam cada tecnologia e por quê".

### 3. Conceitos essenciais com o "por que" de cada um
Mínimo **5 linhas por conceito**; cada conceito ligado ao problema que resolve (ex.: imagem→consistência, container→
isolamento, volume→persistência). Analogias bem-vindas, sempre ancoradas na técnica.

### 4. Guia de uso em DOIS caminhos
Sempre mostrar **CLI/PowerShell** E **ferramenta gráfica/IDE** (Docker Desktop, SSMS, VS Code, portais cloud...), em tabela
comparativa ação × caminho, com o que acontece "por baixo dos panos". Apontar **as páginas mais importantes da documentação
oficial** da ferramenta gráfica (ex.: docs.docker.com/desktop/use-desktop/container/, .../images/, .../volumes/).

### 5. Exemplo operável (nunca hello-world)
Um serviço que **fica vivo** e permite investigar: Nginx antes de SQL Server; container de SQL antes de cluster. Comandos
explicados linha a linha (o que cada flag/porta/volume faz).

### 6. Roteiro de investigação
Comandos práticos de exploração da plataforma para o operador rodar sozinho (inspecionar JSON, medir tamanho, ver camadas,
entrar no container, ver recursos) + os mesmos passos na GUI.

### 7. Gancho para o próximo passo
Fechar instruindo a investigar: "assim que se sentir confortável com X, vamos dar um passo além: [próximo passo concreto do
roadmap]" — **não** encerrar com "testa aí e me conta" sem apontar o destino seguinte.

## Parte 3 — Protocolo de backlog (`DECCO-BACKLOG.md` no projeto)

O backlog vive **no repositório do projeto** (raiz), nunca na skill (Regra de ouro 12). Ele acumula dois tipos de entrada:

1. **`[FEEDBACK]` de resposta:** o operador sinaliza `[FEEDBACK]` + o que refatorar → a skill refatora e registra:
   ```markdown
   ## YYYY-MM-DD — Tema
   ### [FEEDBACK] Título
   **O que foi pedido:** ...
   **Resposta original:** ...
   **Refatoração:** ...
   **Origem:** sessão <id>/data
   ```
2. **Achados do pipeline (T1-T2):** tecnologia introduzida no projeto que encaixa no roadmap mas não estava contemplada →
   registrar como "tecnologia × roadmap" com o tier onde encaixaria, e oferecer incorporar na skill (opt-in).

O operador consome o backlog para **propor versionamento da skill** ("versionar a skill com os achados") — a skill então
transforma as entradas em `knowledge-drops/` + `reference/` + regras novas (exatamente como esta referência nasceu).

## Parte 4 — Referências cruzadas

| Preocupação | Onde |
|---|---|
| Rubrica de sinais por tier/track | `reference/17` §2 |
| Tabela tecnologia → tier | `reference/11` §tabela-resumo |
| Tiers reais (Tier-0 modelo-template, Tier-1 login) | `knowledge-drops/024` |
| Diagnóstico completo sob pedido | `recipes/05` |
| Imutabilidade da skill + estado no projeto | Regra de ouro 12, `knowledge-drops/014` |
| Exemplo do modelo didático aplicado (Docker) | `DECCO-BACKLOG.md` (projeto) + esta sessão |

## Fio a puxar

O T1 infere o estado por **diffs**; o `recipes/05` valida por **rubrica completa**. Quando divergirem, a rubrica precisa de
novos sinais de diff — rastrear as divergências em 3 sessões é o próximo refinamento do pipeline.
