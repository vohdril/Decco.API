# 11 — Foco em camadas (agora vs depois) + a visão lúdica de escala

> ⚠️ Atualização (2026-07-25): Tiers redefinidos a partir dos **projetos reais** taggeados como `modelo-template`.
> Ver `knowledge-drops/024` para a redefinição completa. O Tier-0 agora é o **modelo-template existente**, não mais uma
> construção aspiracional. O Tier-1 é o **problema integrador do login** que cruza todos os tracks.

> Esta referência resolve o risco que o Paulo levantou: *"colocar muita informação de uma vez faz perder o foco e vira um
> projeto espelhado sem estudo"*. Aqui delimitamos **o que importa AGORA** vs o que fica **documentado para depois** — mantendo
> a **linha de estudo não-destrutiva** (um checkpoint funcional por vez, nunca gerar tudo de uma vez).

## A visão (a estrela-guia que dá sentido a cada tecnologia)
**Decco** é, hoje, um banco pequeno (catálogo de anomalias). Mas o **objetivo final** é uma **operação em escala global**, como a
Omnibees é para hotelaria — só que para **anomalias/entidades paranormais**. A imagem mental:

> *"Se os Homens de Preto (MIB) — ou a Fundação SCP — precisassem de um sistema para **registrar e operar tudo** do trabalho
> deles (catalogar anomalias, entidades, artefatos, locais, eventos, incidentes, perícias, contenção), em **muitos sítios pelo
> mundo, com muitos agentes ao mesmo tempo** — Decco seria esse sistema."*

Essa visão **justifica** cada escolha técnica: cache/réplicas existem porque há **escala de leitura global**; eventos existem
porque um **incidente Sigma** num sítio precisa notificar outros; observabilidade existe porque operação 24/7 global precisa ser
vista. **Mas nada disso é necessário no primeiro dia.** O truque é construir por camadas, cada uma um ganho de estudo real.

## O mapa de foco (3 tiers — redefinidos)

### 🟢 TIER 0 — MODELO-TEMPLATE (o que já existe, taggeado)
Objetivo: ter **dois repositórios funcionais** que qualquer pessoa pode clonar e ver rodando. É o checkpoint **antes** de
qualquer feature real. Os repositórios estão em:

- `C:\Decco\git\Decco.API\v0.0.1` — tag `modelo-template`
- `C:\Decco\git\Decco.Dashboard\v.0.0.1` — tag `modelo-template`

**O que o Tier-0 contém:**
- **Decco.API:** ASP.NET 8 Minimal API + EF Core + Dapper híbrido + envelope Request/Response + Contracts/Domain/Infra
- **Decco.Dashboard:** Vite + React 19 + Module Federation 2.0 (`@module-federation/enhanced` 2.8.0) + Radix Dialog + TanStack Query
- **mf-remote (dentro da API):** Vite + React 19 + `@module-federation/vite` 1.19.1, expõe `./DashboardWidget`
- **Micro-frontend operacional:** `DashboardPage.tsx` carrega `decco-demo` via `loadRemote` em `MicroFrontendModal.tsx`
- **Seam mock↔live:** `gateway.ts` + `httpApi.ts` + toggle por env
- **Git tags:** `modelo-template` pusheadas em ambos os repositórios

**Fora do Tier-0:** Docker, autenticação JWT, login real, permissões, Redis, Kafka, Foundation.API, observabilidade, critérios de busca, cache distribuído, multi-scope, K8s.

Checkpoint: `git clone → npm install → npm run dev` abre o dashboard com o micro-frontend carregando do back.

### 🟡 TIER 1 — LOGIN DE VERDADE (o problema integrador)
Quando o Tier-0 está estável, o próximo checkpoint é **implementar autenticação real** — e isso obriga a **tocar todos os 4
tracks simultaneamente** (o que torna o aprendizado mais rico):

| Track | O que muda | Tecnologia |
|---|---|---|
| **DB** | Docker compose com SQL Server + tabelas `User`/`Role`/`UserSite` | Docker Desktop, `docker-compose.yml`, EF Migrations |
| **BE** | `PasswordHasher` (PBKDF2), `POST /api/Auth/token`, `AddJwtBearer`, `ApiPermission` seed, policies | ASP.NET 8 Auth, `JwtSecurityToken`, `IdentityModel` |
| **FE** | `LoginPage.tsx` real com formulário → JWT → `AuthenticatedFetch` → logout | React Context, JWT decode, `Authorization: Bearer` |
| **GT** | Conventional Commits + commitlint + husky + gitleaks | `husky`, `commitlint`, `gitleaks` |

> O Tier-1 **não** existe isolado por track — é um exercício integrador. Cada peça depende da outra:
> - O banco precisa estar em Docker para o back conectar
> - O back precisa servir JWT para o front logar
> - O front precisa exibir o login para o usuário
> - O Git precisa registrar tudo com qualidade

Checkpoint: *"faço login com usuário/senha, o back valida no banco (Docker), recebo JWT com claims, e a UI filtra o que vejo pelo meu clearance/sítio"*.

### 🔵 TIER 2+ — ESCALA (cache, critérios, Foundation, observabilidade)
Cada item aqui ganha sentido pela visão MIB/SCP global — e cada um tem um dossiê na skill:

- **Cache distribuído** (FusionCache L1+L2+Redis backplane) + **rate limiting** — *por quê: muitos agentes lendo hot-data*.
- **Criteria** (query declarativa paginada/filtrada) do `OB.Api.Base.DataLayer` — busca/paginação/filtro.
- **Foundation.API** consumindo a Decco.API por HTTP (o "fio", envelope + paginação 1↔0-based) — exercício de fachada.
- **Observabilidade** (OpenTelemetry + Serilog) + **Docker/compose** (aprimorado) — *por quê: operar 24/7 e ver o sistema*.
- **Eventos/mensageria** (Kafka/MassTransit) — *por quê: um sítio notifica os outros*.
- **Multi-scope / bounded contexts** — *por quê: crescer sem virar monólito*.
- **Auth rica em 2 camadas + Vault + K8s/HPA** — *por quê: segurança e elasticidade globais*.

> Checkpoint por item; nunca todos juntos.

## Regra de ouro do foco (linha não-destrutiva)
1. **Um tier por vez; um checkpoint funcional antes de avançar.** Nunca gerar o projeto inteiro de uma vez.
2. **O que é de tier futuro fica DOCUMENTADO (nas `reference/`) mas NÃO gerado.** Consultar ≠ construir.
3. **Ao gerar algo do Tier 0/1, marcar os ganchos de escala** com `// >>> (escala, Tier 2: ver reference/NN)` — o lugar existe, a implementação espera.
4. **A visão lúdica entra como motivação, não como escopo** — explicar "por que isto importaria num Decco global" sem construir o global agora.
5. **Tier-0 não se refaz.** Se o projeto já subiu para v0.0.1 (modelo-template), não reescrever o Tier-0. O próximo passo é sempre v0.2.0 com login.

## Tabela-resumo (tecnologia → tier → onde está documentado)
| Tecnologia/padrão | Tier | Dossiê na skill |
|---|---|---|
| Scaffold .NET 8 + Vite + React 19 | 🟢 0 | `knowledge-drops/024` |
| Envelope Request/Response | 🟢 0 | `reference/10` |
| EF Core + Dapper híbrido | 🟢 0 | `reference/09`, `reference/14` |
| Module Federation 2.0 (micro-frontend) | 🟢 0 | `knowledge-drops/024` |
| Seam mock↔live (`gateway.ts` + `httpApi.ts`) | 🟢 0 | `reference/16` |
| Modal Radix + 4 estados (Skeleton/Loading/Empty/Error) | 🟢 0 | `reference/16` |
| Docker compose + SQL Server container | 🟡 1 | `knowledge-drops/024`, `reference/18` |
| Auth JWT (`AddJwtBearer`, `POST /api/Auth/token`) | 🟡 1 | `reference/12`, `reference/13` |
| PasswordHasher PBKDF2 | 🟡 1 | `reference/12` |
| Tabelas `User`/`Role`/`UserSite` | 🟡 1 | `reference/13`, `reference/18` |
| Login real + JWT no front | 🟡 1 | `reference/16` |
| Conventional Commits + husky + commitlint | 🟡 1 | `reference/GT` (em assets/frontend-tier0) |
| Gitleaks / scan de segredos | 🟡 1 | `knowledge-drops/001` |
| Cache (Redis / FusionCache) | 🔵 2 | `reference/06`, `reference/09` |
| Criteria (paginação/filtro declarativa) | 🔵 2 | `reference/09` |
| Foundation.API (fachada HTTP) | 🔵 2 (despriorizada) | `reference/03`, `reference/06` |
| Observabilidade (OTel/Serilog) | 🔵 2 | `reference/08` |
| Eventos/Kafka | 🔵 2 | `reference/06` |
| Multi-scope / bounded contexts | 🔵 2 | `reference/08` |
| Vault + K8s/HPA | 🔵 2 | `reference/06` |

> Tudo registado; nada perdido. O foco imediato é a **transição v0.0.1 → v0.2.0**: implementar o **Tier-1 integrado (login real)**.
