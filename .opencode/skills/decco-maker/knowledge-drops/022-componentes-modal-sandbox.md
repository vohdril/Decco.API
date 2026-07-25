# 022 — Modal de Componentes: catálogo interativo no sandbox Decco

- **Data de registo:** 2026-07-24
- **Fonte:** Análise do site de componentes MUI (Material UI All Components) + identidade visual Decco (tokens, components.css, robôs SVG)
- **Tipo:** padrão novo
- **Afeta:** `SKILL.md` (tabela "Como usar"), `reference/16-frontend-tiers-e-estetica.md`, `assets/frontend-tier0/`
- **Camada:** front-end

## Contexto: o que o MUI faz

O [MUI All Components](https://mui.com/material-ui/all-components/) organiza **60+ componentes** em 8 categorias:

| Categoria | Exemplos |
|---|---|
| Inputs | Button, Checkbox, Slider, Switch, TextField, Select, Rating… |
| Data Display | Avatar, Badge, Chip, Table, Tooltip, Typography… |
| Feedback | Alert, Dialog, Progress, Skeleton, Snackbar… |
| Surfaces | Accordion, AppBar, Card, Paper |
| Navigation | Drawer, Menu, Pagination, Tabs, Breadcrumbs… |
| Layout | Box, Container, Grid, Stack |
| Lab | Masonry, Timeline |
| Utils | Modal, Popover, Portal, Transitions… |

Cada componente tem:
- **Live demo interativo** com código editável (sandbox `@codesandbox/sandpack`)
- **Variantes** lado a lado (cor, tamanho, estado disabled/loading)
- **Toggle de props** (ex.: `disableElevation`, `color="secondary"`)
- **Customização** com CSS ou override de tema
- **API docs** (tabela de props, CSS classes, ref)

O que **não** faz (e não precisa): abrir páginas separadas para cada componente — tudo in-page com abas/seções.

## O que o Decco deve implementar

Modal de "Componentes" acessível pelo robô de controle (botão no `RobotDashRefinado` ou nav item) que lista **todos os componentes do design system Decco** com mini live demos.

### Categorias propostas

```
┌─ Inputs ──────────────────────────────┐
│  Button (4 variantes, cores, loading)  │
│  Input / TextField (estados, ícones)   │
│  Select                               │
│  Checkbox / Switch / Radio             │
│  Slider (contínuo, discreto, range)    │
│  Rating                               │
├─ Data Display ────────────────────────┤
│  Badge (cores, dot, contador)          │
│  Chip (removível, variantes)           │
│  Avatar (iniciais, imagem, tamanhos)   │
│  Tooltip (posições, atraso)            │
│  Divider                               │
│  Typography (escala, cores, mono/sans) │
├─ Feedback ────────────────────────────┤
│  Alert (severidade, ícone, dismiss)    │
│  Progress (linear, circular, valor)    │
│  Skeleton (texto, card, avatar)        │
│  Toast / Snackbar (sucesso, erro)      │
├─ Navigation ──────────────────────────┤
│  Tabs (Radix, ícones, contador)        │
│  Pagination                            │
│  Breadcrumbs                           │
│  Menu / Dropdown                       │
├─ Surface ─────────────────────────────┤
│  Card (básico, com ações, grid)        │
│  Accordion (simples, múltiplo)         │
├─ Data Viz ────────────────────────────┤
│  Chart (linha inline SVG, sparkline)   │
│  Sphere (canvas 2D interactivo)        │
│  StatTile (valor, label, severidade)   │
├─ Utils ───────────────────────────────┤
│  Modal (tamanhos, composição)          │
│  LockedFeature (tier bloqueado layout) │
│  DeviationWarning                      │
└───────────────────────────────────────┘
```

### Layout da modal

Cada categoria é um **acordeão** ou **aba** (Radix Accordion/Tabs). Dentro:
- **Grid de cards** (2-3 colunas responsivo)
- Cada card tem:
  - Nome do componente + badge de categoria
  - **Mini live demo inline** (SVG/CSS puro, sem dependências externas)
  - Props toggle: botões para alternar variante/cor/estado
  - Código fonte abrevia do ao lado (ou em `<details>`)

### Padrão de interatividade

Cada mini demo deve:
1. **Responsivo a hover** — ex.: Button muda cor no hover, Card levanta
2. **Troca de cor em tempo real** — botões de toggle entre `--accent`, `--core`, `--sev-*`
3. **Troca de estado** — normal / disabled / loading / error
4. **Animação suave** — `transition: all .15s ease` (padrão Decco existente)
5. **Código visível** — `<pre><code>` colapsável ao lado

### Exemplo: mini demo de Button

```tsx
// Estado local no modal
const [btnColor, setBtnColor] = useState<"accent" | "core" | "danger">("accent");
const [btnLoading, setBtnLoading] = useState(false);

<demo>
  <div className="demo__row">
    <button className={`btn btn--${btnColor}`} disabled={btnLoading}>
      {btnLoading ? "Carregando…" : "Ação"}
    </button>
    <button className={`btn btn--${btnColor} btn--outline`} disabled={btnLoading}>
      Secundária
    </button>
  </div>
  <div className="demo__controls">
    <button onClick={() => setBtnColor("accent")}>Accent</button>
    <button onClick={() => setBtnColor("core")}>Core</button>
    <button onClick={() => setBtnColor("danger")}>Danger</button>
    <label><input type="checkbox" onChange={e => setBtnLoading(e.target.checked)} /> Loading</label>
  </div>
</demo>
```

### Identidade visual (mantendo o Decco atual)

Tudo deve respeitar os tokens existentes:
- Fundo: `--bg-0/1/2/3`
- Acento: `--accent` / `--accent-strong` / `--accent-glow`
- Núcleo: `--core`
- Severidade: `--sev-pacato/yaguara/abaporu/ukar`
- Tipografia: `--font-mono` para código/labels, `--font-sans` para corpo
- Borda: `--border` / `--border-strong`
- Sombras: `--shadow`, `--focus-ring`
- Raio: `--r-sm/md/lg`
- Transições: `all .15s ease` (hover), `fade-in .2s ease` (modais)

## Porquê

O **MUI All Components** é referência de catálogo interativo porque:
1. **Live demos reduzem atrito** — o utilizador vê o componente funcionando sem abrir outra página
2. **Toggle de props ensina** — alternar cor/estado mostra o efeito imediato sem ler doc
3. **Grid de cards é scanneável** — encontra o componente que precisa em segundos

O Decco já tem design system próprio (tokens, componentes CSS, robôs SVG). A modal de Componentes completa o sandbox:
- **Ensina** os componentes disponíveis para quem for estender a UI
- **Centraliza** o catálogo (hoje espalhado entre ComponentsSandboxPage, modal de usuário, GlossarioPage)
- **Demonstra** a identidade visual Decco sem precisar de dependências externas (MUI, Chakra, etc.)

Alternativa rejeitada: importar MUI ou Chakra UI como dependência — adiciona bundle, foge do propósito didático do sandbox (criar do zero). Alternativa rejeitada: página separada por componente (como MUI faz para os complexos) — o Decco não precisa, os componentes são simples o suficiente para caber in-modal.

## Como aplicar a partir de agora

1. Criar `src/pages/ComponentesModal.tsx` (componente de modal, consumido pelo `DashboardPage` ou pelo robô)
2. Estruturar como **Radix Dialog** (`<Dialog.Root>`), reusando estilos `modal__*` já existentes em `components.css`
3. Usar **Radix Accordion** para as categorias
4. Cada demo é um componente leve inline (sem deps externas, só CSS modules + tokens)
5. Os dados dos componentes (nome, categoria, demo component) viver em `src/data/componentes.ts`
6. O robô abre a modal via `onComponentesClick` prop (análogo ao `onCoreClick`)

## Fio a puxar (opcional)

Explorar se o `ComponentsSandboxPage` atual (sandbox de componentes) deve ser absorvido pela modal ou continuar existindo como página de testes mais complexa. A modal seria o catálogo "polido"; a SandboxPage o "playground editável".
