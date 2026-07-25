# 012 — Front-end real: stack Tier-0, estados de UI, authz na UI e a esfera

- **Data de registo:** 2026-07-17
- **Fonte:** decisão de design + implementação executável a pedido do Paulo (projeto React real em `Downloads/DeCCo/front`, validado por `npm run build` limpo e render conferido no browser).
- **Tipo:** padrão novo (abre a trilha de FE, antes só esboçada)
- **Afeta:** `reference/16-frontend-tiers-e-estetica.md` (novo), `assets/frontend-tier0/` (novo asset), `open-questions/Q-006` (resolve a parte "framework"), `reference/11` (paralelo de tiers), `reference/13` (espelho da auth rica na UI)
- **Camada:** front-end (novo track, paralelo ao core/fachada)

## O padrão / a mudança
A skill passa a ter uma **trilha de front-end por tiers** (FE0–FE4) e carrega uma **implementação de referência FE Tier-0
executável** em `assets/frontend-tier0/` — o análogo de `assets/decco.sql`: a "fotografia" que qualquer ambiente copia,
`npm install && npm run dev`, e vê de pé, **sem back-end** (roda sobre **mocks do Tier-0**).

**Stack FE Tier-0 (decidido):** Vite + React 19 + TypeScript · react-router-dom 7 · **@radix-ui/react-dialog** (modais) ·
**@tanstack/react-query** (server-state) · **react-hook-form + zod** (forms) · lucide-react · clsx · **esfera em canvas 2D próprio**.

**O que a referência já entrega (as disciplinas de um sistema escalável, não um "hello world"):**
- **Login + rotas protegidas** com **tipos de usuário** (clearance/sítio) — espelho visual de `reference/13`.
- **Estados de UI de fábrica**: Skeleton · Loading/Spinner · Empty · Error — com botões de **cenário** (OK/Lento/Vazio/Erro) que os forçam para estudo.
- **Modais acessíveis** (Radix Dialog): detalhe, criar (validado) e confirmar exclusão.
- **Autorização a nível de recurso NA UI**: `visibleTo(user, anomalia)` filtra por **clearance + sítio** — trocar de operador muda a lista.
- **Sandbox de componentes editável** (array `REGISTRY`) — documentação viva do design system.
- **Design system JARVIS** com **tokens CSS** como fonte única (re-tematiza o app inteiro sem tocar em componente).
- **Esfera de partículas** + gráfico SVG inline + KPIs.

## Porquê (obrigatório nesta skill)
- **Radix Dialog > `<dialog>` nativo** (a pergunta original do Paulo): o nativo obriga a fazer foco/scroll-lock/ARIA/portal à
  mão e tem **teto baixo** para a operação continental pretendida; Radix é **headless + acessível de fábrica** e **veste-se** com
  a estética. Alternativa de escala (shadcn/ui) é **a mesma base** (Radix+Tailwind) — logo a escolha não é um beco.
- **TanStack Query** em vez de `useEffect`+`useState`: loading/erro/cache/refetch **declarativos**; sem reinventar corridas e
  invalidação. É o **padrão** para estado de servidor.
- **Isolar o acesso a dados atrás de uma interface** (`mocks/api.ts` com a assinatura de um cliente HTTP): trocar mock→API real
  vira mudança **local** (só o corpo do `api.ts`), a UI não sente. É o que "escalável" significa na prática — e o gancho do FE Tier 1.
- **Canvas 2D para a esfera no Tier 0**: **zero dependência** → instala/roda leve; o upgrade para **r3f + shaders** é o FE Tier 3,
  sem mudar a interface do componente (por isso a esfera é um componente isolado).
- **Autossuficiência**: carregar o projeto como **asset** (não só descrevê-lo) é o mesmo princípio de `decco.sql` — o FE precisa
  ser reproduzível **em qualquer ambiente/modelo**, sem depender desta conversa (foi o teste de validação que o Paulo pediu).

## Como aplicar a partir de agora
- Pedido de front → abrir `reference/16`. **Regra não-destrutiva vale igual ao back:** um FE tier por vez; o FE Tier-0 é o
  console mínimo "de produção" (login + estados + modais + authz-na-UI + sandbox), não um CRUD cru.
- **Não** puxar three.js/data-grid/gráficos pesados/micro-frontends para o Tier 0 — ficam documentados em `reference/16` (FE 2/3/4).
- Ao ligar no back (FE Tier 1), **só** reescrever o corpo de `mocks/api.ts` (manter `types.ts`); pré-requisito no back = **OpenAPI + CORS**.
- Deixar claro ao operador: **authz na UI é defense-in-depth, não segurança** — a verdade é no back (`reference/13`).
- Estender componentes sempre via `REGISTRY` (sandbox) + token/primitivo, nunca estilo inline solto.

## Fio a puxar
Gerar o **SDK-cliente TS por NSwag** da Decco.API e trocar **um** endpoint do mock pelo real — medir o que muda na UI (deve ser
~nada além de `api.ts`). Primeiro passo real do **FE Tier 1** e prova do isolamento de dados. Micro-frontends seguem **abertos**
em `Q-006` (estudo exploratório, sem análogo Omnibees confirmado).
