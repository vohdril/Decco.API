# 024 — Tier-0 Definitivo: os projetos reais e o problema do Login

> ⚠️ Atualização definitiva do conceito de Tiers (2026-07-25). Substitui a visão abstrata de `reference/11` por uma **realidade concreta**: dois repositórios funcionais, taggeados como `modelo-template`, com micro-frontend operacional via Module Federation 2.0. O Tier-0 agora significa **"modelo-template"**, e o problema que obriga a cruzar todos os 4 tracks é a **tela de login**.

## Contexto

Até este drop, os Tiers eram um **mapa aspiracional** — descrições do que *um dia* seria construído. Agora **existe**:

| Repositório | Tag | Estado |
|---|---|---|
| `C:\Decco\git\Decco.API\v0.0.1` | `modelo-template` | ASP.NET 8 Minimal API + EF Core + Dapper + serve `mf-remote/dist` via StaticFiles |
| `C:\Decco\git\Decco.Dashboard\v.0.0.1` | `modelo-template` | Vite + React 19 + Module Federation 2.0 (`@module-federation/enhanced` + `@module-federation/vite`) carregando `decco-demo` remoto |

### O que o modelo-template tem

**Decco.API (v0.0.1):**
- Solução com `Decco.Api.REST` (Minimal API), `Decco.Contracts`, `Decco.Domain`, `Decco.Infra`
- EF Core 8 + Dapper híbrido no mesmo DbContext
- Envelope `RequestBase<T>/ResponseBase<T>` com `Single<T>/List<T>/Paged<T>`
- `Program.cs` serve `mf-remote/dist` em `/mf-remote` com `UseStaticFiles`
- `.gitignore` cobre `node_modules/` e `mf-remote/node_modules/`
- Connection string em `appsettings.json`

**Decco.Dashboard (v0.0.1):**
- Vite + React 19 + TypeScript + react-router-dom 7
- `@module-federation/enhanced` 2.8.0 (host) + `@module-federation/vite` 1.19.1 (host)
- `MicroFrontendModal.tsx` com `loadRemote` do `decco-demo` (strip do `./` no exposedModule)
- `DashboardPage.tsx` com remote config `{name:"decco-demo", entry: "${API_BASE}/mf-remote/remoteEntry.js", exposedModule: "./DashboardWidget"}`
- `Modal.tsx` com Radix Dialog, `primitives.tsx` com Skeleton/Loading/Empty/Error
- `tierContent.ts` com conteúdo de todos os tiers (FE0-4, BE0-4, DB0-4, GT0-3)
- `TierRoadmap.tsx` com mapa de progressão

**mf-remote (dentro de Decco.API):**
- Vite + React 19 + `@module-federation/vite` 1.19.1
- Expõe `./DashboardWidget` com `DashboardWidget.tsx` importado no `src/main.tsx`
- Build: `mf-remote/dist/remoteEntry.js`

### O que NÃO tem (v0.0.1 → v0.2.0)

1. Docker em qualquer lugar
2. Autenticação (JWT, hash de senha, login endpoint)
3. Tela de login no dashboard (só mock)
4. Permissionamento (clearance, sítio, roles)
5. Banco rodando em container
6. Serviços e repositórios reais separados (lógica hoje está solta)
7. Health check endpoint real
8. CORS configurado para dev

## A redefinição: Tiers são agora exercícios integradores

O problema central para transição de v0.0.1 → v0.2.0 é:

> **"Precisamos implementar a tela de login corretamente, com JWT, banco Docker, serviços no Decco.API e permissões."**

Cada Track contribui com uma peça. **Todos os 4 tracks sobem um degrau simultaneamente** — não é um tier por track isolado.

| Track | v0.0.1 (modelo-template) | v0.2.0 (login real) | Ferramentas |
|---|---|---|---|
| **FE** | Mock login + mock auth context | Tela de login real com JWT, `AuthenticatedFetch`, logout | React Context, JWT decode, `Authorization` header |
| **BE** | Estrutura de projeto + contracts | `POST /api/Auth/token`, `PasswordHasher`, `AddJwtBearer`, serviços | ASP.NET 8 Auth, PBKDF2, JwtSecurityToken |
| **DB** | Schema SQL apenas | Docker SQL Server + `User`/`Role`/`UserSite` tabelas | Docker Desktop, `docker-compose.yml`, EF migrations |
| **GT** | Git init + tags | Conventional Commits, CI via GitHub Actions, scan de segredos | `husky`, `commitlint`, `gitleaks`, `semantic-release` |

### Nova definição do Tier-0

> **Tier-0 = `modelo-template`.** É o checkpoint estável antes de qualquer feature real. Deve ser clonável, `npm install && npm run dev`, e mostrar o dashboard com o micro-frontend carregando do back.

| Sinal | Descrição | Presente em v0.0.1? |
|---|---|---|
| Projeto .NET 8 rodando | `dotnet run` abre a API | ✅ |
| Projeto Vite rodando | `npm run dev` abre o dashboard | ✅ |
| Micro-frontend visível | Dashboard mostra widget do `decco-demo` | ✅ |
| Seam mock↔live | `data/gateway.ts` + `data/httpApi.ts` + toggle | ✅ |
| Modal Radix funcional | Detalhe/criar/excluir com Radix Dialog | ✅ |
| Envelope Request/Response | Contracts com `RequestBase`/`ResponseBase` | ✅ |
| EF Core + Dapper híbrido | DbContext + `SqlRepositoryBase` + SP via Dapper | ✅ |
| Tag `modelo-template` | Git tag pushada | ✅ |

### Nova definição do Tier-1 (v0.2.0)

> **Tier-1 = login de verdade.** O projeto ganha autenticação, Docker e permissões. O checkpoint é: *"faço login com usuário/senha, o back valida no banco (Docker), recebo um JWT com claims, e a UI filtra o que vejo pelo meu clearance/sítio"*.

| Track | O que implementar | Sinal decisivo |
|---|---|---|
| **DB (parte)** | Docker compose com SQL Server + tabelas `User`/`Role`/`UserSite` | `docker-compose.yml` na raiz + `docker ps` mostra container SQL |
| **BE (parte)** | `PasswordHasher` (PBKDF2), `POST /api/Auth/token` (valida credenciais → JWT), `AddJwtBearer` no pipeline | `AddJwtBearer` em `Program.cs` + endpoint `/api/Auth/token` |
| **FE (parte)** | `LoginPage.tsx` real com formulário → chama `/api/Auth/token` → armazena JWT → `AuthenticatedFetch` com `Authorization: Bearer` | `useAuth` real (não mock), `ProtectedRoute` com JWT decode |
| **BE (parte 2)** | `ApiPermission` seed, authorize endpoints por policy, filtro por clearance/sítio | `[Authorize(Policy = "Clearance5")]` ou equivalente |
| **GT (parte)** | Conventional Commits + commitlint + husky | `.husky/commit-msg` + `commitlint.config.js` |

### Tiers 2-4 mantêm-se como escala

- **Tier-2**: Cache (Redis/FusionCache), Criteria (paginação declarativa), Catálogos CRUD, Foundation.API
- **Tier-3**: Observabilidade (OTel/Serilog), Eventos (Kafka), Multi-scope
- **Tier-4**: K8s, Vault, Elasticsearch, réplicas RO

## Implicações para a skill

1. `reference/11` deve refletir a nova tríade: **Tier-0 = modelo-template** (o que existe), **Tier-1 = login real** (próximo checkout), **Tiers 2-4 = escala** (futuro).
2. `reference/16` (front-end) deve adicionar FE1 como "Login real + JWT", não "ligar no back" genérico.
3. `reference/18` (database) deve ter DB0.5 como "Docker + tabelas de auth".
4. `reference/17` (tracking) deve detectar se a auth foi implementada antes do micro-frontend (fora-de-sequência válido).
5. `recipes/04-frontend-tier0.md` deve guiar o login real como próximo passo.
6. O `tierContent.ts` e `TierRoadmap.tsx` no Dashboard devem refletir a redefinição.
7. Ao receber pedidos de feature, a skill deve primeiro verificar se o projeto está no `modelo-template` (Tier-0) ou já evoluiu.

## Diagrama de progressão (Tier-0 → v0.2.0)

```
Tier-0 (modelo-template, v0.0.1)
  ├── FE: Dashboard + Micro-frontend + Mock auth
  ├── BE: Estrutura + Contracts + EF/Dapper
  ├── DB: Schema SQL (LocalDB)
  └── GT: Git init + tag modelo-template
        │
        ▼  Problema integrador: "implementar tela de login"
        │
Tier-1 (v0.2.0)
  ├── FE: Login real + JWT + AuthenticatedFetch + Logout
  ├── BE: Auth endpoint + PasswordHasher + JWT middleware + ApiPermission
  ├── DB: Docker compose + SQL Server container + User/Role/UserSite
  └── GT: Conventional Commits + husky + gitleaks
        │
        ▼  Próximo problema integrador (Tier-2)
        │
Tier-2 (v0.3.0)
  ├── FE: FE2 (TanStack Table + Recharts)
  ├── BE: Redis/FusionCache + Criteria + Foundation.API
  ├── DB: Catálogos CRUD + DB1 endpoints
  └── GT: semantic-release + CI/CD
```

## Como referenciar este drop

- **Situação**: "o projeto está no modelo-template" → `knowledge-drops/024` §Contexto
- **O que fazer agora**: subir para v0.2.0 com login → `knowledge-drops/024` §Nova definição do Tier-1
- **Dúvida sobre Tier-0 vs Tier-1**: consultar a tabela de sinais em §Nova definição do Tier-0
