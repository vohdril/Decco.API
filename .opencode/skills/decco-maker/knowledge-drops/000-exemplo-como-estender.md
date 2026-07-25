# 000 — Exemplo: como estender esta skill (modelo de formato)

- **Data de registo:** 2026-07-12
- **Fonte:** criação da skill decco-maker
- **Tipo:** padrão novo
- **Afeta:** —
- **Camada:** ambos

## O padrão / a mudança
Este ficheiro é só um **molde**. Um drop real documenta um padrão/decisão/correção concreto do sandbox Decco — por exemplo,
"decidimos que `Decco.Contracts` é um projeto referenciado (não um pacote NuGet)", ou "o repositório híbrido usa `QueryMultiple`
para `sp_Anomalia_ObterPerfilCompleto`".

## Porquê (obrigatório nesta skill)
Diferente das skills partners-*, aqui **todo** drop explica a razão de desenho, a alternativa rejeitada e o que quebraria de
outro jeito. É o que torna a skill didática: o leitor futuro entende o *porquê*, não só o *quê*.

## Como aplicar a partir de agora
Ao registar um drop, preencher **todos** os campos do template do `INDEX.md`, especialmente **Porquê**. Numerar em sequência
e atualizar o índice. Correção nunca apaga a base — só anota `⚠️ Atualização`.

## Fio a puxar (opcional)
Quando um drop amadurece e vira regra estável, vale promovê-lo para `reference/` ou `recipes/`. Como decidir a hora certa?
