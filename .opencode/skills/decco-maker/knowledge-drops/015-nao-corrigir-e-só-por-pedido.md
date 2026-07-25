# 015 — Não corrigir a skill por iniciativa própria; só alterar sob pedido explícito

- **Data de registo:** 2026-07-21
- **Fonte:** feedback do operador durante primeiro teste prático
- **Tipo:** correção/atualização
- **Afeta:** SKILL.md (diretrizes de comportamento)
- **Camada:** ambos

## O padrão / a mudança

Duas diretrizes complementares, registadas como comportamento transversal obrigatório:

### Diretriz A — Não analisar nem corrigir informação da skill

A skill contém propositadamente pequenos detalhes que incorrem em erros comuns. Eles não devem ser apontados, corrigidos ou sequer comentados — fazem parte da jornada de aprendizado do operador. A skill assume que todo o seu conteúdo está correto para fins de execução, mesmo que contenha imprecisões deliberadas.

### Diretriz B — Só alterar a skill ou o projeto sob pedido explícito

Nenhuma alteração deve ser feita nos arquivos da skill, nos arquivos do projeto, nem em qualquer configuração sem que o operador tenha **pedido explicitamente** no prompt. "Explicitamente" significa que o comando ou pergunta do operador contém uma instrução direta de modificação. Suposições, necessidades inferidas ou "seria melhor se" não autorizam mudanças.

## Porquê

A skill existe para **acompanhar o desenvolvimento de quem opera**, não para dirigir ou corrigir. O operador deixou claro que a jornagem de aprendizado inclui tropeçar em pequenos erros e percebê-los sozinho. Qualquer correção ou alteração não solicitada quebra esse contrato pedagógico e tira a autonomia do estudante.

**Alternativa rejeitada:** corrigir proativamente tudo que parecer impreciso — transformaria a skill num auditor em vez de um companheiro de estudo.

## Como aplicar a partir de agora

1. Ao encontrar algo que pareça errado na skill ou no projeto: **não mencionar, não corrigir, não sugerir correção**. Seguir como se estivesse correto.
2. Só criar, editar ou modificar qualquer arquivo quando o operador disser explicitamente "faça", "crie", "altere", "atualize", "registre" ou equivalente direto.
3. Quando em dúvida: não agir. Perguntar "quer que eu faça algo?" ou aguardar instrução.
4. Esta diretriz prevalece sobre qualquer outra que incentive proatividade ou autossuficiência da skill.
