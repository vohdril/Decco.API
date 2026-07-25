# 017 — Frontend DB2: sidebar contraível, glossário, tabs anomalia, dashboard hero

- **Data de registo:** 2026-07-22
- **Fonte:** Pedido do operador para enriquecer experiência visual do Dashboard
- **Tipo:** complemento
- **Afeta:** SKILL.md (versionamento → v3), reference/16 (frontend tiers)
- **Camada:** Frontend (Dashboard React/Vite)

## O padrão / a mudança

### Sidebar contraível
- Sidebar com botão toggle (`PanelLeftOpen`/`PanelLeftClose`) no topo
- Expandida (200px): brand, categorias "NAVEGAÇÃO" + "CONFIGURAÇÕES" com labels, ícone + label, chip do usuário
- Contraída (68px): apenas ícones com tooltip (title)
- Grid layout: `grid-template-columns` alterna entre `68px 1fr` / `200px 1fr` com transição CSS
- Categoria CONFIGURAÇÕES contém: Cognição, Periculosidade, Laboratórios, Protocolos

### Glossário
- Página `/glossario` que consome dados mock de `CLASSES`, `CAMADAS`, `MECANISMOS`
- Classes: cards com cor de borda pela `corAlerta` (Pacato=verde, Yaguara=âmbar, Abaporu=vermelho, Ukar=roxo)
- Camadas: cards com cores por camada (Theta=cyan, Psi=roxo, Phi=verde, Omega=laranja)
- Mecanismos: tabela com badge colorido por camada

### Modal de anomalias em 3 tabs (Radix Tabs)
- Tab "Informações": grid 2 colunas com todos os campos + placeholder de imagem (`ImageOff` + "Nenhuma imagem disponível")
- Tab "Manifestações": EmptyState com `Radar` (placeholder para tier futuro)
- Tab "Estatísticas": EmptyState com `BarChart3` (placeholder para tier futuro)

### Dashboard hero
- Mensagem "Bem-vindo ao DeCCO" com lore do departamento
- Robô SVG redesenhado: cabeça-monitor com olhos ovais digitais (inspirado em Eva de WALL-E), tronco+quadril separados, braços longos, pernas curtas
- Grid de tiers (FE0 a FE4) lado a lado com status (✅/▶/🔒) e itens desbloqueados

## Porquê
- Sidebar contraível libera espaço horizontal sem perder navegação
- Glossário didático mostra dados de catálogo com identidade visual do DeccoDB
- Tabs organizam informação crescente da anomalia sem poluir o modal
- Dashboard com mensagem + robô + tiers dá contexto lúdico e informativo ao operador

## Versão da skill
- SKILL.md atualizada para v3
