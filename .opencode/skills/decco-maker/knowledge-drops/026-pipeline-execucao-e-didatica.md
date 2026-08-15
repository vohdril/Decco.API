# 026 — Pipeline de execução tier-aware + modelo didático de respostas (skill v11)

- **Data de registo:** 2026-07-29
- **Fonte:** Sessão Decco.API v0.0.1 — pedido explícito do operador para versionar a skill com: (1) pipeline de execução com noção de Tier anterior/atual/próximo; (2) validação em background a cada invocação via git diff × RoadMaps; (3) backlog alimentado por tecnologias não contempladas; (4) modelo didático de respostas calibrado na resposta refatorada de Docker; (5) seção "Problemas que [tecnologia] resolve" enriquecida com pesquisa de mercado + documentação oficial.
- **Tipo:** padrão novo (complementa Regra de ouro 13 e Diretriz 2)
- **Afeta:** `SKILL.md` (novas regras 15/16 e diretriz 11), `reference/17`, novo `reference/20`, `templates/DECCO-PROGRESS.md`, `knowledge-drops/024`
- **Camada:** todos (transversal — não é código de API, é meta-camada de condução)

## O padrão: o pipeline de execução em 4 tempos

Toda invocação da skill passa a seguir um **pipeline de execução** com 4 tempos, antes de responder o pedido:

```
T1. VALIDAÇÃO EM BACKGROUND (silenciosa, barata)
    git status/diff do repo + artefatos-âncora → detecta o que MUDOU desde o último contato
T2. QUANTIFICAÇÃO DO ESTADO (por track, independente)
    cruza os diffs com a rubrica de sinais (reference/17) → Tier observado por track
T3. ORIENTAÇÃO (anterior → atual → próximo)
    reporta "de onde veio / onde está / para onde vai" e detecta conflitos de escopo
T4. EXECUÇÃO DO PEDIDO
    responde seguindo o modelo didático (abaixo) e, se aplicável, alimenta o DECCO-BACKLOG.md
```

### O que muda em relação à skill atual

| Antes (v10) | Agora (v11) |
|---|---|
| Diagnóstico só quando pedido ("rode um diagnóstico") | **Validação em background a cada invocação** — o estado é sempre fresco |
| Tier derivado só por rubrica estática | Tier derivado por **git diff incremental** × rubrica × tecnologias esperadas do roadmap |
| `DECCO-PROGRESS.md` declarado, reconciliado sob pedido | **Trio anterior/atual/próximo** sempre narrado; o observado vence |
| Fora-de-sequência = aviso pontual | Fora-de-sequência = **sinal de estudo** (progresso assimétrico entre tracks orienta o que estudar) |
| Backlog não existe | **`DECCO-BACKLOG.md` no projeto** — alimentado quando a skill encontra tecnologia que encaixa no roadmap mas não estava contemplada |

### Progresso assimétrico entre tracks (o que estudar mais/menos)

A possibilidade de um track estar à frente de outro **não é erro — é informação de bússola**:
- **FE à frente de BE** → o estudo agora pede BE (o operador domina o front; falta o back para integrar).
- **DB à frente de tudo** → pede consumir o schema (BE/FE), não modelar mais.
- O pipeline reporta a **assimetria como recomendação de estudo**: "FE0 completo e BE0 em x/y → próximo foco: fechar BE0 (repo-ef, repo-dapper-sp...) antes de subir ambos para Tier-1".

### Validação em background: o que rodar (ordem barata → cara)

1. `git status --short` + `git log --oneline -5` + `git tag` — mudanças desde o último contato, sem abrir arquivos.
2. `git diff --stat HEAD` quando houver mudanças não commitadas — *quais* áreas mudaram (contratos? UI? SQL?).
3. Cruzar os paths alterados com a tabela **tecnologia esperada → tier** (`reference/11` §tabela-resumo):
   - path novo de tecnologia esperada do tier corrente → **progresso** (marcar checkpoint);
   - path novo de tecnologia de tier FUTURO sem o corrente fechado → **fora-de-sequência** (aviso didático);
   - path novo de tecnologia **não contemplada no roadmap** mas que encaixa → **registrar no `DECCO-BACKLOG.md`** como "tecnologia introduzida × roadmap" e oferecer incorporar na skill (opt-in).
4. Só então, se o pedido exigir, abrir arquivos (`reference/17` §rubrica) para confirmar sinais decisivos.

**Custo:** os passos 1-3 são 3 comandos git — a skill pode (e deve) rodá-los **sempre**, em background, sem anunciar, exceto quando o resultado mudar o rumo (progresso novo, conflito de tier, tecnologia nova).

## O modelo didático de respostas (a referência de abstração)

A resposta refatorada de Docker desta sessão é o **molde de referência** para explicar qualquer tecnologia. Seções obrigatórias, nesta ordem:

1. **O ecossistema X (camadas)** — as peças da tecnologia e o papel de cada uma, antes de qualquer comando.
2. **Problemas que [tecnologia] resolve (antes de qualquer comando)** — a seção mais rica: o problema histórico, a alternativa rejeitada (o que quebraria sem ela), **e a visão de mercado**: que tipos de solução/operação demandam a tecnologia no mundo corporativo e por quê. Requer pesquisa em docs oficiais (docs.docker.com, learn.microsoft.com...) + posicionamento do produto (ex.: docker.com/resources/what-container) — citar as fontes.
3. **Conceitos essenciais com "por que" de cada um** — mínimo 5 linhas por conceito; cada um ligado ao problema que resolve.
4. **Guia de uso em DOIS caminhos** — CLI/PowerShell **e** a ferramenta gráfica/IDE (Docker Desktop, VS Code, SSMS...), sempre com tabela comparativa ação × caminho; apontar as páginas mais importantes da documentação oficial.
5. **Exemplo operável** — nunca hello-world; algo que fique vivo e permita investigar (Nginx > SQL Server > ...), com comandos explicados linha a linha.
6. **Roteiro de investigação** — comandos práticos de exploração da plataforma (inspecionar, medir, entrar, ver logs) para o operador rodar sozinho.
7. **Gancho para o próximo passo** — "assim que se sentir confortável, vamos dar um passo além: [próximo passo concreto]" — encerrar instruindo a investigar, não apenas "testa aí".

## O protocolo [FEEDBACK] e o DECCO-BACKLOG.md

- O operador sinaliza **`[FEEDBACK]`** quando quer refatoração de uma resposta; a skill então **registra no `DECCO-BACKLOG.md`** (raiz do projeto): o que foi pedido, a resposta original, a refatoração aplicada e a origem (sessão/data).
- O `DECCO-BACKLOG.md` acumula também **tecnologias introduzidas que encaixam no roadmap mas não estavam contempladas** (achados do T1/T2) — é o canal que alimenta futuras versões da skill.
- O backlog vive no **projeto** (não na skill — Regra de ouro 12 preservada). O operador pode consumi-lo para propor versionamento da skill ("implementar um versionamento na skill" — como este drop).

## Porquê (obrigatório nesta skill)

- **Validação em background** resolve o problema de contexto: a skill passa a saber o estado do projeto **sem depender de memória de conversa** — reforça a Regra de ouro 13 ("a frio") e torna o acompanhamento contínuo, não pontual.
- **Alternativa rejeitada:** rodar diagnóstico completo a cada mensagem (caro, polui o diálogo). O pipeline T1-T2 é incremental e silencioso; só fala quando há mudança de rumo.
- **Trio anterior/atual/próximo** responde ao risco de regressão: sem ele, uma interação pode gerar código de Tier 2 num projeto de Tier 1 sem ninguém perceber. Com ele, o conflito vira alerta antes da geração.
- **Progresso assimétrico como bússola** é a alternativa didática ao "tudo junto": tiers são guias por track, não trilhos — o desnível diz ao operador onde investir estudo.
- **Backlog no projeto** (não na skill) preserva a imutabilidade padrão da skill (Regra de ouro 12) e dá ao operador um artefato versionável que ele mesmo consome para propor novas versões.
- **Seção "Problemas que resolve" com visão de mercado** atende o pedido explícito: explicações não podem ser só técnicas — devem dizer **que soluções e operações demandam a tecnologia e por quê** (ex.: Docker é demandado por CI/CD, microservices, cloud portability, padronização de ambientes de time).

## Como aplicar a partir de agora

1. **Toda invocação:** rodar T1 (3 comandos git) em background; anunciar só se houver mudança de rumo.
2. **Respostas de tecnologia:** seguir as 7 seções do modelo didático; a seção 2 exige pesquisa em docs oficiais + mercado, citando fontes.
3. **Sempre que detectar:** (a) progresso novo → marcar checkpoint; (b) conflito de tier → aviso didático não-bloqueante; (c) tecnologia nova não contemplada → propor registro no `DECCO-BACKLOG.md`.
4. **`[FEEDBACK]` do operador:** refatorar a resposta e registrar no backlog (o quê/antes/depois/origem).
5. **Ao versionar a skill:** consumir o `DECCO-BACKLOG.md` como fonte dos achados (como este drop fez).

## Fio a puxar

Quanto o T1 (git diff incremental) antecipa em relação a um diagnóstico completo? Medir: rode a validação em background em 3 sessões diferentes e compare o estado inferido só pelos diffs com o veredito de um `recipes/05` completo — onde divergem, a rubrica precisa de mais sinais de diff.
