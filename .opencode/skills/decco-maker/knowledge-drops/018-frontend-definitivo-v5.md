# 018 — Estado definitivo Tier-0 (v5): Dashboard completo, API CRUD, banco com lore brasileiro

- **Data de registo:** 2026-07-22
- **Fonte:** Sessão completa de desenvolvimento do zero até o estado atual, sob pedido explícito do operador
- **Tipo:** atualização/correção (substitui fotografia de referência dos assets)
- **Afeta:** SKILL.md (versionamento → v5), assets/frontend-tier0/ (substituído), assets/decco.sql (substituído)
- **Camada:** Frontend (Dashboard React/Vite) + Backend (Decco.API .NET 8) + Banco (DeccoDB)

## O estado atual (fotografia definitiva do Tier-0)

### Frontend (assets/frontend-tier0/)
Projeto Vite+React 19+TS executável em modo mock ou conectado à Decco.API.

**Estrutura completa (56 source files):**
```
src/
├── App.tsx                       — Rotas: /login, /, /anomalias, /glossario, /config/*
├── auth/auth.tsx                 — Auto-login "vance" (clearance 4, role DIRETOR_SITIO), localStorage, ProtectedRoute
├── data/
│   ├── gateway.ts                — Interface DeccoApi (contrato público: 52 métodos)
│   ├── httpApi.ts                — Implementação HTTP real (fetch)
│   ├── index.ts                  — Seam: export api = httpApi (troque para mockApi se quiser modo off-line)
│   └── apiPaths.ts               — Paths dos endpoints REST
├── mocks/
│   ├── types.ts                  — 14 tipos: Anomalia, ClasseObjeto, CamadaOntologica, User, DashboardStats etc.
│   ├── data.ts                   — Dados mock (classes, camadas, anomalias, usuários, etc.)
│   └── api.ts                    — Implementação mock (não ativa por padrão)
├── lib/queryClient.ts            — TanStack Query client
├── components/
│   ├── layout/AppShell.tsx       — Sidebar (85px colapsada / 200px expandida), NAV + CATÁLOGOS, user chip
│   ├── robot/
│   │   ├── RobotAnatomy.tsx      — SVG decomposto em 15 componentes nomeados + 3 glows + RobotDashCompleto
│   │   ├── ParticleSphere.tsx    — Canvas 2D com 900 pontos, Fibonacci sphere, ondulação, rAF loop
│   │   └── robot_dash_pre-alpha.svg — SVG original de referência
│   └── ui/
│       ├── primitives.tsx        — Panel, StatTile, Button, Skeleton, LoadingState, ErrorState, EmptyState
│       ├── Modal.tsx, Toast.tsx, Sphere.tsx
│       ├── TierRoadmap.tsx       — FE/BE/DB tiers progressivos (FE0-4, BE0-3, DB0-4)
│       ├── LockedFeature.tsx     — Functionalidade bloqueada com safety rails
│       └── DeviationWarning.tsx  — Safety rail para desvios de estudo
├── pages/
│   ├── DashboardPage.tsx         — RobotDashSVG inline, health check (listForcasFundamentais), 3 kanbans
│   ├── GlossarioPage.tsx         — RobotDashCompleto + ParticleSphere + 4 queries (Cogn, Peric, Forc, Mec)
│   ├── AnomaliasPage.tsx         — CRUD anomalias (grid de cards com hover)
│   ├── CognicaoPage.tsx          — CRUD Cat_CognicaoAparente
│   ├── PericulosidadePage.tsx    — CRUD Cat_Periculosidade
│   ├── LaboratorioPage.tsx       — CRUD Laboratorio
│   ├── ProtocoloPage.tsx         — CRUD ProtocoloContencao
│   ├── InstanciaDeviantePage.tsx — CRUD Instancia_PericiaDesviante
│   ├── ManifestacaoEspecificaPage.tsx — CRUD Cat_ManifestacaoEspecifica
│   ├── MecanismoInteracaoPage.tsx     — CRUD Cat_MecanismoInteracao
│   ├── PericiaAnomaliaPage.tsx   — CRUD PericiaAnomalia
│   ├── LoginPage.tsx             — Tela de login mock
│   └── ComponentsSandboxPage.tsx — Galeria de componentes
├── styles/
│   ├── tokens.css                — Design tokens Navy/Ciano/Âmbar
│   ├── global.css                — Reset, body, scrollbar, panel, layout utilities, animações
│   ├── components.css            — Todos os estilos de componentes + AppShell + modal + toast + login + esfera
│   └── user-overrides.css        — Overrides manuais (margem sidebar, nav items)
├── main.tsx                      — Entry point com providers
├── vite.config.ts                — Vite config
├── package.json                  — Dependências (React 19, TanStack Query 5, Radix, rhf+zod, lucide)
├── tsconfig.json                 — TypeScript config
├── index.html                    — HTML entry
└── .env.example                  — VITE_API_BASE_URL=http://localhost:5000
```

**Design System:**
- `--bg-0` navy escuro (#060b12) → `--bg-3` (#12202f)
- `--accent` ciano (#34d3e6), `--accent-strong` (#6ff0ff)
- `--core` âmbar (#ffb347)
- 4 severidades: Pacato#4CAF50, Yaguara#FFC107, Abaporu#F44336, Ukar#9C27B0
- `--font-mono`: JetBrains Mono / Cascadia Code
- Sidebar: `shell__side--expanded` (margin-left: 4px, width calc(100% - 16px))

**Componentes-chave do Dashboard:**
- RobotDashSVG inline com `<g name="robot-skeleton">` imutável
- Glows: olho-esquerdo (cx=183.1, cy=33.7), olho-direito (cx=274.4, cy=33.7), interior-tronco (cx=229, cy=184)
- Health check usa `api.listForcasFundamentais()` como query de sincronização
- 3 estados: loading (LoadingState), error (robô 210px + "Erro ao sincronizar"), success

**Componentes-chave do Glossário:**
- 4 queries paralelas com LoadingState/ErrorState + retry por painel
- RobotDashCompleto + ParticleSphere centralizados no topo
- Grids Classes/Camadas: `repeat(auto-fill, minmax(280px, 1fr))` gap 14px
- Cards com hover: translateY(-2px), shadow, border-color var(--accent)

**Dependências (package.json):**
```json
{
  "@hookform/resolvers": "^3.10.0",
  "@radix-ui/react-dialog": "^1.1.6",
  "@radix-ui/react-tabs": "^1.1.18",
  "@tanstack/react-query": "^5.90.2",
  "clsx": "^2.1.1",
  "lucide-react": "^0.469.0",
  "react": "^19.2.0",
  "react-dom": "^19.2.0",
  "react-hook-form": "^7.54.2",
  "react-router-dom": "^7.12.0",
  "zod": "^3.24.1",
  "devDeps": {
    "@vitejs/plugin-react": "^5.1.1",
    "typescript": "~5.9.3",
    "vite": "^7.2.4"
  }
}
```

### Backend (Decco.API .NET 8)
Solution com 6 projetos, arquitetura em camadas, envelope Request/Response.

```
Decco.sln
├── Decco.Contracts/              — DTOs + Envelope (RequestBase, SingleResponse, BulkResponse, PagedResponse)
├── Decco.Api.Common/             — IService, IRepository (interfaces base)
├── Decco.Api.Contracts/          — ErrorCodes.cs
├── Decco.Api.DataLayer/          — DeccoDbContext (22 DbSets), Models/*, Repositories/*
├── Decco.Api.Services/           — Services (I*/Service para cada entidade)
├── Decco.Api.Operations/         — AnomaliaManager (operações de domínio)
├── Decco.Api.Root/               — CompositionRoot (DI por convenção: RegisterByConvention)
└── Decco.Api.REST/               — Controllers, Program.cs, appsettings.json
```

**Program.cs:**
- PID handshake (kill-decco.ps1)
- CORS: localhost:5173 (configurável)
- EF Core + SQL Server (LocalDB)
- DI via RegisterDependencies()

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DeccoDb": "Server=(localdb)\\MSSQLLocalDB;Database=DeccoDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:5173"]
  }
}
```

**Controllers (padrão):**
- Rota: `api/[controller]`
- 5 endpoints POST: List, Get, Insert, Update, Delete
- Envelope: `RequestBase<T>` → `SingleResponse<T>`
- 10 controllers: Anomalia, CatForcaFundamental, CatCamadaOntologica, CatCognicaoAparente, CatPericulosidade, CatMecanismoInteracao, CatTipoMaterium, Laboratorio, ProtocoloContencao, NotificacaoAnomalia

**Repositories (17):**
- IAnomaliaRepository (CRUD + Dapper)
- 7 catálogos: ICatForcaFundamentalRepository, ICatCamadaOntologicaRepository, ICatClasseObjetoRepository, ICatCognicaoAparenteRepository, ICatPericulosidadeRepository, ICatMecanismoInteracaoRepository, ICatTipoMateriumRepository
- Laboratorio, ProtocoloContencao, NotificacaoAnomalia

### Banco (DeccoDB — assets/decco.sql)
1346 linhas, database-first, SQL Server/LocalDB.

**8 tabelas de catálogo:**
Cat_ClasseObjeto (PACATO/YAGUARA/ABAPORU/UKAR), Cat_ForcaFundamental (Kappa/Lambda), Cat_CamadaOntologica (THETA/PSI/PHI/OMEGA), Cat_TipoMateria, Cat_MecanismoInteracao, Cat_ManifestacaoEspecifica, Cat_CognicaoAparente (SE/SA/IN/AA), Cat_Periculosidade (níveis 1-9)

**14 tabelas de negócio:**
Anomalia (seed 1000), EntidadeViva, Artefato, Localidade, Evento, Laboratorio, ProtocoloContencao, Protocolo_AplicadoEm, NotificacaoAnomalia, PericiaAnomalia, Instancia_PericiaDesviante (polimórfica), Pericia_Manifestacao (N:N), Incidente, Vw* (views)

**3 stored procedures:** sp_Anomalia_Inserir, sp_Anomalia_Atualizar, sp_Anomalia_Buscar (com paginação)

**2 triggers:** TR_Anomalia_Update_Date, TR_Anomalia_Validar_Mecanismos

## Versão da skill
- SKILL.md atualizada para **v5**
- assets/frontend-tier0/ substituído pela fotografia exata do Dashboard v.0.0.1
- assets/decco.sql substituído pela versão 1346 linhas
- Esta é a **versão definitiva** — toda geração futura de Tier-0 deve produzir este estado exato
