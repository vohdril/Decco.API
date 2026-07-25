# 019 — Dashboard v0.0.2: Modais de Tier com Hello World Explicativo

## Contexto

Após mapear o progresso completo dos 3 projetos (Dashboard, Decco.API, DeccoDB)
em um diagnóstico a frio, identificou-se a necessidade de tornar o roadmap do
Dashboard mais informativo: cada card de tier (FE0..FE4, BE0..BE3, DB0..DB4)
deveria abrir um modal explicativo.

## Entregas

- **Dashboard v0.0.2** (commit `9199f45`, tag `v0.0.2`)

### Arquivos novos

- `src/data/tierContent.ts` — acervo com 14 tiers cobertos:
  - Intro explicativa (1-2 parágrafos sobre o propósito do tier)
  - Bloco de código hello world real da tecnologia

### Arquivos modificados

- `TierRoadmap.tsx` — aceita `onTierClick?: (tier: TierNode) => void` opcional.
  Nodes ganham `role="button"`, `tabIndex={0}`, e classe `roadmap__node--clickable`
  quando o callback é fornecido.

- `DashboardPage.tsx` — adiciona:
  - `useState<TierNode | null>(null)` para o tier selecionado
  - Componente `TierModal` que renderiza `<Modal>` com título, intro e `<pre>` com código
  - `onTierClick={setSelectedTier}` em cada `<TierRoadmap>`

- `DashboardPage.module.css` — `.tier-modal-body`, `.tier-modal-intro`, `.tier-modal-code`

- `components.css` — `.roadmap__node--clickable` com `cursor: pointer` e hover glow

### Conteúdo dos tiers

| ID   | Tecnologia              | Hello world                                  |
|------|-------------------------|----------------------------------------------|
| FE0  | React + Vite            | `createRoot` + `<h1>`                        |
| FE1  | Fetch + httpApi         | `fetch(/api/health)`                         |
| FE2  | @tanstack/react-table   | `createColumnHelper` + `useReactTable`       |
| FE3  | Three.js + R3F          | `<Canvas>` + `<mesh>`                        |
| FE4  | Module Federation       | `ModuleFederationPlugin` config              |
| BE0  | .NET 8 Minimal API      | `MapGet(/api/health)`                        |
| BE1  | EF Core CRUD            | `MapGet` + `ToListAsync`                     |
| BE2  | JWT + RBAC              | `AddJwtBearer` + `TokenValidationParameters` |
| BE3  | Redis / FusionCache     | `AddFusionCache` + `GetOrSetAsync`           |
| BE4  | Kafka / OpenTelemetry   | `AddOpenTelemetry` + `AddCap`                |
| DB0  | SQL DDL                 | `CREATE TABLE Anomalia`                      |
| DB1  | Stored Procedures       | `CREATE PROCEDURE sp_Cognicao_Inserir`       |
| DB2  | Joins N:N               | `CREATE VIEW vw_AnomaliaClasses`             |
| DB3  | Full-Text / Elasticsearch | `CREATE FULLTEXT INDEX`                    |
| DB4  | CDC / Polyglot          | `sys.sp_cdc_enable_table`                    |

## Skill Assets Atualizados

- `assets/frontend-tier0/` sincronizado com o estado v0.0.2:
  - `src/data/tierContent.ts` (novo)
  - `src/components/ui/TierRoadmap.tsx`
  - `src/pages/DashboardPage.tsx`
  - `src/pages/DashboardPage.module.css`
  - `src/styles/components.css`
