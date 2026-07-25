# 16 — Front-end: trilha por tiers, stack e estética (Terminal de Contenção)

> ⚠️ Atualização (2026-07-25): FE0 agora reflete o **modelo-template real** com Module Federation 2.0. FE1 redefinido como
> **"Login real + JWT"** em vez de "ligar no back" genérico (o seam já existe no Tier-0). Ver `knowledge-drops/024`.

> Espelha o mesmo **método não-destrutivo** do back-end (`reference/11`): **um tier por vez, um checkpoint funcional antes de
> subir**, o resto **documentado mas não gerado**. A skill carrega uma **implementação de referência Tier-0 executável** em
> `assets/frontend-tier0/` (o análogo de `assets/decco.sql` para o front) — é a "fotografia" que qualquer ambiente pode copiar,
> `npm install && npm run dev`, e ver de pé.

## A visão (por que um front, e por que assim)
Se o **Decco** é o sistema de uma operação **MIB/SCP em escala global** (`reference/11`), o front-end é o **console de operação**:
o posto onde um agente **cataloga anomalias, vê incidentes Sigma em tempo real, filtra por sítio e gera relatórios**. Isso
justifica cada escolha: **estados de carregamento/vazio/erro** existem porque a rede real falha e demora; **autorização a nível
de recurso na UI** existe porque *nem todo agente vê toda anomalia* (`reference/13`); **modais/notificações acessíveis e
escaláveis** existem porque a operação sai de "listar 4 anomalias" para "notificar e relatar milhares de ocorrências
continentais". **Nada disso é necessário no primeiro dia** — entra por camadas, cada uma um ganho de estudo real.

## O mapa de foco (FE tiers — redefinidos)

### 🟢 FE TIER 0 — CONSOLE MOCK + MICRO-FRONTEND (modelo-template)
Objetivo: um **dashboard executável e autossuficiente** que já carrega um **micro-frontend real via Module Federation 2.0**.
É o que está nos repositórios taggeados como `modelo-template`:
- **Decco.Dashboard (host):** Vite + React 19 + `@module-federation/enhanced` 2.8.0 + `@module-federation/vite` 1.19.1
- **mf-remote (dentro da Decco.API):** Vite + React 19, expõe `./DashboardWidget` via `remoteEntry.js`
- **Micro-frontend carregando:** `MicroFrontendModal.tsx` com `loadRemote` + `DashboardPage.tsx` com remote config apontando para `API_BASE/mf-remote/remoteEntry.js`
- **Login mock:** `auth/auth.tsx` com `ProtectedRoute` + `useAuth` (usuários simulados: vance/thoth/o5)
- **Estados de UI:** Skeleton, Loading, Empty, Error (`primitives.tsx`) + botões de cenário
- **Modais acessíveis:** `Modal.tsx` com Radix Dialog + forms com react-hook-form + zod
- **Server-state:** TanStack Query (`useQuery`/`useMutation`)
- **Seam mock↔live:** `gateway.ts` + `httpApi.ts` + toggle por env `VITE_DATA_SOURCE`
- **Autorização na UI:** `visibleTo(user, anomalia)` por clearance + sítio
- **Esfera de partículas:** `Sphere.tsx` canvas 2D
- **Sandbox de componentes:** `ComponentsSandboxPage.tsx` com `REGISTRY`
- **TierContent completo:** dados de todos os tiers (FE0-4, BE0-4, DB0-4, GT0-3)

> **Fora do FE Tier 0:** login real com JWT, cliente HTTP ligado ao back (o esqueleto existe; o flip é FE1), SDK NSwag,
> data-grid virtualizado, gráficos Recharts, notificações Sonner, 3D com shaders, i18n, testes E2E.

Checkpoint: *"npm run dev abre o dashboard, o micro-frontend carrega do back, vejo anomalias mock, abro modal Radix, forço os 4 estados"*.

### 🟡 FE TIER 1 — LOGIN REAL + JWT (v0.2.0)
O Tier-0 tem login **mock**. O Tier-1 substitui por autenticação real:
- **LoginPage real:** formulário que chama `POST /api/Auth/token` no back, recebe JWT, armazena em memória (context) + localStorage
- **AuthenticatedFetch:** wrapper de `fetch` que injeta `Authorization: Bearer <token>` em toda requisição
- **JWT decode:** extrai claims (name, role, clearance, site) do token para o `useAuth` context
- **Logout:** limpa token, redireciona para login
- **Token refresh** (ou re-login ao expirar): tratamento de 401 com redirect
- **ProtectedRoute real:** verifica se token existe E não está expirado (não só se "loggedIn" mock)
- **Flip do seam:** `data/httpApi.ts` ganha headers de auth; `VITE_DATA_SOURCE=live` funciona com back real
- **SDK-cliente (opcional):** NSwag/OpenAPI Generator a partir do Swagger do back

> O FE Tier 1 não existe isolado — depende do BE Tier 1 (endpoint `/api/Auth/token`) e do DB Tier 1 (Docker + tabelas de auth).
> É um **exercício integrador** que cruza os 3 tracks. Ver `knowledge-drops/024`.

Checkpoint: *"digito usuário/senha na tela de login, o back valida no banco Docker, recebo JWT, o dashboard filtra pelo meu clearance"*.

### 🔵 FE TIER 2 — TABELAS E GRÁFICOS
- **Data-grid** escalável (**TanStack Table**) — ordenação/coluna/virtualização para milhares de ocorrências.
- **Gráficos** (**Recharts** para o comum; **visx/D3** quando precisar de controle fino) — substituem o SVG inline do Tier 0.
- **Notificações em escala** — trocar o Toast próprio por **Sonner** ou **@radix-ui/react-toast** (fila, swipe, a11y, agrupamento).
- **Paginação/filtro** consumindo o contrato paginado (0-based no fio; a UI mostra 1-based — `reference/03`).

### 🟣 FE TIER 3 — 3D E IMERSÃO
- **Esfera com shaders:** migrar `Sphere.tsx` (canvas 2D) para **react-three-fiber + three.js** (GLSL, pós-processamento/bloom).
  A interface do componente **não muda** — é troca de implementação (por isso o canvas foi isolado num componente só).

### ⚫ FE TIER 4 — PLATAFORMA (já atingido pelo modelo-template)
- **Micro-frontends** (Module Federation) — ✅ **já implementado no Tier-0** com `@module-federation/enhanced` + `@module-federation/vite`
- **Design system empacotado** (Storybook + publicação do pacote de UI)
- **Mobile** (React Native/Expo reusando `types`+SDK)

> **Nota:** O micro-frontend (FE4 conceitual) foi implementado já no Tier-0 como parte do exercício de Module Federation.
> É um **fora-de-sequência legítimo e intencional** — a equipe optou por aprender MF cedo. O Tier-4 real incluiria
> **deploy independente, múltiplos remotes, Storybook e mobile**, que ainda são futuros.

## Stack escolhido (FE Tier 0) — decisões com "porquê" e alternativa rejeitada
| Área | Escolha | Porquê / alternativa rejeitada |
|---|---|---|
| Build/base | **Vite + React 19 + TypeScript** | Dev-server instantâneo. *Rejeitado CRA (morto), Next (SSR desnecessário p/ um console interno).* |
| Rotas | **react-router-dom 7** | Padrão de SPA; `ProtectedRoute` simples. |
| Modais/diálogos | **@radix-ui/react-dialog** | **Headless + acessível de fábrica** (focus-trap, `Esc`, scroll-lock, ARIA `role=dialog`, portal). *Rejeitado `<dialog>` nativo* (a11y/foco/portal manuais); *rejeitado MUI/AntD* (tema próprio e peso). |
| Server-state | **@tanstack/react-query** | Cache + loading/erro/refetch **declarativos**; `invalidateQueries` após mutação. |
| Forms/validação | **react-hook-form + zod** | Type-safe, pouco re-render; schema zod serve UI e back. |
| Module Federation | **@module-federation/enhanced 2.8.0 + @module-federation/vite 1.19.1** | Única solução madura para MF com Vite. *Rejeitado Webpack 5 ModuleFederationPlugin* (exige migrar de Vite). |
| Ícones | **lucide-react** | Tree-shakeable, traço fino combina com HUD. |
| Esfera | **canvas 2D próprio** (Tier 0) | **Zero dependência** → o projeto instala e roda leve. Upgrade = **r3f** no FE Tier 3. |

## O design system (estética JARVIS/HUD) — fonte única
Tudo sai de **tokens CSS** em `src/styles/tokens.css` (cores, severidades, tipografia mono). Mudar a paleta ali **re-tematiza o
app inteiro** sem tocar em componente. Camadas: `tokens.css` (variáveis) → `global.css` (base + utilitários HUD: `.panel`,
`.hud-label`, animações) → `components.css` (componentes/páginas). **Severidade por token**: cada classe de anomalia
(`PACATO`/`YAGUARA`/`ABAPORU`/`UKAR`) tem sua cor — o `SeverityBadge` só **lê** o token (`CorAlerta` do DeccoDB → cor), então a
paleta é a única coisa a mexer.

## Disciplinas que o Tier 0 já ensina (e por quê)
- **Skeleton com a forma do conteúdo real** (não um spinner genérico) → elimina *layout shift* e melhora a percepção de velocidade.
- **Empty ≠ Error**: vazio é um **estado de sucesso** (oferece a próxima ação/CTA); erro explica e deixa **retry**. Confundir os
  dois é o erro mais comum de dashboards.
- **Loading como estado de primeira classe** (via Query), não um `if (loading)` espalhado.
- **Micro-frontend com Module Federation**: um remote React 19 dentro de um host React 19, com shared singleton, sem duplicação.
- **Autorização na UI é *defense-in-depth*, não segurança**: `visibleTo` melhora a experiência (não mostra o que não interessa),
  mas **a verdade é no back** (`reference/13`) — a UI nunca é a fronteira de segurança.

## Como rodar / como o asset mapeia
```bash
# Decco.Dashboard
cd C:\Decco\git\Decco.Dashboard\v.0.0.1
npm install
npm run dev        # http://localhost:5173

# Decco.API (serve o mf-remote)
cd C:\Decco\git\Decco.API\v0.0.1
dotnet run         # http://localhost:5000
```

Login demo (mock): `vance` (Pesquisador/Sítio-19) · `thoth` (Diretor/Sítio-64) · `o5` (acesso total). Senha p/ todos: `decco`.
Fonte de dados: **mock** por default; alternar p/ **live** no seletor **DADOS** (topo) ou `VITE_DATA_SOURCE=live` no `.env`.

Para **login real (FE Tier 1)**, o back precisa estar rodando com Docker + auth endpoint — ver `knowledge-drops/024` §Tier-1.

## Como estender (o mesmo espírito didático)
1. **Novo componente** → adicione ao array `REGISTRY` em `ComponentsSandboxPage.tsx` (aparece no catálogo com exemplo vivo).
2. **Novo estado/variação** → primeiro no primitivo (`primitives.tsx`) e no token, nunca inline na página.
3. **Ligar no back (auth real)** → implemente `POST /api/Auth/token` no back + `AuthenticatedFetch` no front + `VITE_DATA_SOURCE=live`.
4. **Marcar ganchos de tier futuro** com `// >>> (FE Tier N: ...)`, igual ao back.
5. **Múltiplos micro-frontends** → adicione novas entries no `vite.config.ts` (remotes) e novos `loadRemote` nos modais.

## Fio a puxar
Depois de implementar o login real (FE Tier 1), experimente **dois navegadores simultâneos**: logue como `vance` (clearance 3)
numa janela e como `o5` (clearance 5) noutra. A lista de anomalias visíveis é diferente? O `visibleTo` mock existe, mas
quando vier do back é de verdade. Esse é o momento em que "autorização na UI é defense-in-depth" vira realidade palpável.
