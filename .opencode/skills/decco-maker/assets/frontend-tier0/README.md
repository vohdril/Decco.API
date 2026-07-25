# DeCCo · Front — Terminal de Contenção (estudo)

Front-end de estudo do sandbox **Decco**, estética **JARVIS/HUD**, sobre mocks do **Tier-0** (catálogo de anomalias).
É um **demo autossuficiente** (não precisa de backend) para validar o design system, os estados de UI e os padrões escaláveis.

## Rodar
```bash
npm install
npm run dev      # http://localhost:5173
```
> Login demo — operadores: `vance` (Pesquisador/Sítio-19), `thoth` (Diretor/Sítio-64), `o5` (acesso total). Senha: `decco`.
> Se aparecerem avisos de peer-deps (React 19), `npm install --legacy-peer-deps`.

## Mock ↔ Decco.API (fonte de dados)
O front **não depende do back-end para rodar**. A fonte de dados é alternável:
- **Runtime** — o seletor **DADOS · MOCK · DECCO.API** no topo da tela (grava em `localStorage` e recarrega).
- **Estático** — `VITE_DATA_SOURCE=mock|live` no `.env` (copie de `.env.example`); no modo `live` usa `VITE_API_BASE_URL`.

Como funciona (boas práticas): a UI depende só do **contrato** `DeccoApi` (`src/data/gateway.ts`); há duas implementações — `src/mocks/api.ts` (mock, em memória) e `src/data/httpApi.ts` (HTTP, esqueleto p/ Decco.API) — e um **ponto único** que escolhe qual usar (`src/data/index.ts`). Trocar a fonte **não toca em nenhuma página**. No modo `live` sem o back de pé, as chamadas falham de propósito (é o que exercita o tratamento de erro).

## Stack (escolhas justificadas — corporativa e escalável)
| Área | Lib | Por quê |
|---|---|---|
| Build | **Vite + React 19 + TS** | igual ao seu MyFlix; rápido |
| Rotas | **react-router-dom 7** | padrão |
| Server-state | **@tanstack/react-query** | cache + loading/error declarativos (o padrão p/ dados remotos) |
| **Modais / diálogos** | **@radix-ui/react-dialog** | headless + **acessível** (focus-trap, ESC, scroll-lock, ARIA, portal) — a escolha da pesquisa p/ escala, superior ao `<dialog>` nativo; veste-se com a estética própria |
| Forms/validação | **react-hook-form + zod** | padrão corporativo, type-safe |
| Ícones | **lucide-react** | limpo, tree-shakeable |
| **Fonte de dados** | seam **gateway + factory** | alterna **mock ↔ Decco.API** sem tocar na UI (runtime ou `.env`); upgrade = **MSW** |
| Esfera 3D | **canvas próprio** (Tier-0) | placeholder fiel do orbe; **FE Tier 3** = migrar p/ react-three-fiber + shaders |

**Evoluções por tier (documentadas, não incluídas):** shadcn/ui (Radix+Tailwind) · TanStack Table (grids/relatórios) · Recharts/visx (data-viz) · Sonner ou @radix-ui/react-toast (notificações em escala).

## O que o demo cobre
- **Login** (tipos de usuário/clearance) + rotas protegidas.
- **Skeleton / Loading / Empty / Error** — na página **Anomalias**, botões de *cenário* (OK/Lento/Vazio/Erro) forçam cada estado.
- **Modais** (Radix): detalhe, criar (rhf+zod) e confirmar exclusão.
- **Autorização a nível de recurso** no mock: cada usuário só vê anomalias do seu **clearance + sítio** (entre `vance`, `thoth` e `o5` a lista muda).
- **Sandbox de componentes** (`/componentes`): documentação viva e **editável** — adicione entradas no array `REGISTRY` em `src/pages/ComponentsSandboxPage.tsx`.
- **Esfera de partículas** no dashboard + gráfico SVG inline + KPIs.

## Estrutura
```
src/
  main.tsx App.tsx
  styles/        tokens.css (design system) · global.css · components.css
  lib/           queryClient.ts
  data/          gateway.ts (contrato DeccoApi) · index.ts (factory mock/live) · dataSource.ts (toggle) · httpApi.ts (esqueleto HTTP)
  mocks/         types.ts · data.ts · api.ts   (impl MOCK do gateway; Tier-0)
  auth/          auth.tsx  (context + ProtectedRoute; tipos de usuário)
  components/
    ui/          primitives.tsx · Modal.tsx (Radix) · Toast.tsx · Sphere.tsx
    layout/      AppShell.tsx (sidebar + topbar + seletor DADOS)
  pages/         LoginPage · DashboardPage · AnomaliasPage · ComponentsSandboxPage
```

## Ligar no backend real (FE Tier-1)
Já está tudo isolado atrás do contrato `DeccoApi`. Para ir ao vivo:
1. Suba a **Decco.API/Foundation.API** com **OpenAPI + CORS**.
2. Aponte `VITE_API_BASE_URL` (`.env`) e passe a fonte para `live` (seletor DADOS ou `.env`).
3. Complete `src/data/httpApi.ts` — os `// >>>` marcam onde ajustar caminhos e desembrulhar o envelope. Idealmente, gere o cliente por **NSwag/OpenAPI Generator** e substitua esse arquivo pelas chamadas tipadas.

A UI (`pages/`, `components/`) **não muda** em nenhum dos passos — é o objetivo do seam.
