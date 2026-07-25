# 17 — Tracking de progresso + diagnóstico assertivo (não-destrutivo)

> ⚠️ Atualização (2026-07-25): Novos sinais para o Tier-0 real (modelo-template) e Tier-1 (login). Adicionada detecção de
> **implementações de Tier superior feitas precocemente** (ex.: Module Federation no Tier-0, que conceitualmente é FE4).
> Ver `knowledge-drops/024` para a redefinição completa dos tiers.

> Como a skill sabe **em que Tier o projeto está** e **o que falta**, cruzando o **código-fonte** com uma **rubrica de sinais
> por Tier** — mesmo numa conversa **fria** (primeira vez vendo o repositório, sem contexto). Duas invariantes:
>
> 1. **O estado vive no PROJETO, não na skill.** O progresso fica em `DECCO-PROGRESS.md` na raiz do repositório (versionado no
>    git). A **skill é referência imutável** e **não se autoedita** em nenhum ambiente.
> 2. **O diagnóstico é read-only e não-bloqueante.** Ele **situa e orienta** — nunca reprova, nunca bloqueia, nunca escreve (só
>    atualiza o `DECCO-PROGRESS.md` se o operador **pedir explicitamente**). **O observado (código) vence o declarado.**

## 0. Como rodar (resumo — detalhe operacional em `recipes/05`)
Gatilhos: *"rode um diagnóstico do progresso atual"*, *"em que tier estou?"*, *"analise as implementações X e Y"*.
Passos: **(1)** descobrir track(s) pelos artefatos → **(2)** ler `DECCO-PROGRESS.md` se existir (declarado) → **(3)** checar os
**sinais** da rubrica (§2) no código → **(4)** derivar o **Tier observado** por track (§3) → **(5)** detectar **fora-de-sequência**
(§4) → **(6)** **reconciliar** declarado × observado (§5) → **(7)** emitir o **relatório** (§6). Nunca depende de memória da conversa.

## 1. Descoberta de track (o que procurar primeiro)
| Track | Artefato-âncora que identifica o track |
|---|---|
| **A — Back moderno** (`Decco.API`) | `*.csproj` **SDK-style** com `<TargetFramework>net8.0</TargetFramework>` + `<PackageReference>` |
| **B — Back legado** (`Decco.Legacy.API`) | `*.csproj` **old-style** (`<TargetFrameworkVersion>v4.8</TargetFrameworkVersion>`) + `packages.config` + `.edmx` |
| **C — Front** | `package.json` com `react` + `vite`; pasta `src/` |
| **Vertical Auth** (opt-in) | `AddJwtBearer` / `PasswordHasher` / `ApiPermission` / tabelas `User`/`Role`/`UserSite` |
| **D — DB** | `docker-compose.yml` (DB1+) ou `Cat_CognicaoAparente` existe (DB0) |
| **E — Micro-frontend** (track emergente) | `vite.config.ts` com `federation` plugin ou `package.json` com `@module-federation` |

Cada track tem **Tier observado próprio**; **nunca** somar/mediar entre tracks — reportar lado a lado.

## 2. Rubrica de sinais por Tier (o coração do diagnóstico)
> Cada conceito-checkpoint tem um **id** (usado no `DECCO-PROGRESS.md`), um **sinal decisivo** (o que basta achar no código para
> considerar o conceito presente) e o dossiê-fonte. Sinais completos/secundários: ver a `reference/NN` citada. `(fraco)` = indício
> fraco, não decide sozinho. Opcionais **não contam** no denominador do fechamento do tier (§3).

### Track A — Back moderno (`Decco.API`)
**🟢 Tier 0 — modelo-template** — checkpoint: *"projeto .NET 8 rodando com EF Core/Dapper, envelope, servindo mf-remote"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `envelope` | projeto `Decco.Contracts` com `RequestBase`/`ResponseBase`/`Status`(enum Success/PartialSuccess/Fail)/`Error` + `Single*`/`List*` | ref/10, ref/03 |
| `dbcontext-efcore` | pacote `Microsoft.EntityFrameworkCore.SqlServer` 8.x + 1 `DbContext` + 1 `DomainScope` (`Decco`) | ref/09, ref/11 |
| `repo-ef` | repositório com CRUD por EF/LINQ (`SqlRepositoryBase<T>`) | ref/09 |
| `repo-dapper-sp` | pacote `Dapper` + `sp_Anomalia_Buscar` via `CommandType.StoredProcedure` sobre `ctx.Database.GetDbConnection()`; POCO sufixo `*QR` | ref/14 |
| `host-minimal` | `Program.cs` minimal hosting + controllers + DI nativa | ref/06, ref/09 |
| `serve-mf-remote` | `app.UseStaticFiles()` servindo `mf-remote/dist` em `/mf-remote` | knowledge-drops/024 |
| `git-tag-template` | `git tag` contém `modelo-template` | knowledge-drops/024 |

**🟡 Tier 1 — login real (v0.2.0)** — checkpoint: *"auth JWT funcional com Docker, endpoint de token, tabelas de usuário"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `docker-compose` | `docker-compose.yml` na raiz com `services.sql` (SQL Server) | knowledge-drops/024, ref/18 |
| `auth-jwt` | `AddJwtBearer` em `Program.cs` + `TokenValidationParameters` | ref/12, ref/13 |
| `auth-endpoint` | `POST /api/Auth/token` que valida credenciais e emite JWT | ref/12 |
| `password-hasher` | `PasswordHasher`/PBKDF2 (não `AddIdentity`) | ref/12 |
| `user-table` | Entidade `User` (EF code-first) + `Role` + `UserSite` com seed `HasData` | ref/13, ref/18 |
| `api-permission` | `ApiPermission` seed + deny-by-default (endpoint sem permissão → 403) | ref/12 |
| `conventional-commits` | `.husky/commit-msg` + `commitlint.config.js` na raiz | knowledge-drops/024 |

**🔵 Tier 2 — escala (um item = um checkpoint; nunca todos juntos)**
| id | Sinal decisivo | Doc |
|---|---|---|
| `cache-fusion` | `ZiggyCreatures.FusionCache` + `StackExchange.Redis` | ref/06 |
| `criteria` | `ACriteriaBase`/`AQueryableCriteriaBase` (`PageIndex/PageSize/Filter/Orders`) + `ApplyPagedCriteriaAsync` | ref/09 |
| `foundation-api` | Projeto `Foundation.API` separado com `CallApiAsync` + paginação 1→0-based | ref/03, ref/06 |
| `observabilidade` | `OpenTelemetry`(OTLP) + `Serilog.Sinks.Graylog` | ref/06 |
| `eventos-kafka` | `MassTransit.Kafka` + `Confluent.SchemaRegistry.Serdes.Avro` | ref/06 |
| `multi-scope` | vários `DomainScope`/DbContext (Catálogo/Operações/Incidentes) | ref/11, ref/10 |

### Track C — Front (React/Vite)
**🟢 FE0 — console mock + micro-frontend (modelo-template)** — checkpoint: *"dashboard rodando com MF remoto, modais Radix, 4 estados"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `seam-dados` | `src/data/gateway.ts` (interface `DeccoApi`) + `src/data/index.ts` (factory) + `data/httpApi.ts` | ref/16 |
| `login-mock` | `src/auth/auth.tsx` (`ProtectedRoute`, `useAuth`, Context) + `LoginPage.tsx` | ref/16 |
| `micro-frontend` | `vite.config.ts` com `@module-federation/enhanced` + `MicroFrontendModal.tsx` / `loadRemote` | knowledge-drops/024, ref/16 |
| `dashboard` | `src/pages/DashboardPage.tsx` + `TierRoadmap.tsx` + `Sphere.tsx` | ref/16 |
| `estados-4` | `src/components/ui/primitives.tsx` (Skeleton/Loading/Empty/Error) + botões de cenário | ref/16 |
| `modais-radix` | dep `@radix-ui/react-dialog` + `src/components/ui/Modal.tsx` | ref/16 |
| `tier-content` | `src/data/tierContent.ts` com FE/BE/DB/GT tiers | ref/16 |

**🟡 FE1 — login real + JWT** — checkpoint: *"login com JWT real, AuthenticatedFetch, logout, ProtectedRoute real"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `login-real` | `LoginPage.tsx` chama `POST /api/Auth/token` (não só mock `signIn`) | ref/16 §FE1 |
| `jwt-context` | `useAuth` real com `jwtDecode`, armazenamento de token, `isExpired` | ref/16 §FE1 |
| `auth-fetch` | Wrapper `fetch` que injeta `Authorization: Bearer <token>` | ref/16 §FE1 |
| `logout` | Botão/rota que limpa token e redireciona para `/login` | ref/16 §FE1 |

**🔵 FE2 — tabelas e gráficos:** `@tanstack/react-table` / `recharts`|`@visx` / `sonner`.
**🟣 FE3 — 3D:** `three`+`@react-three/fiber` (esfera com shaders).
**⚫ FE4 — plataforma:** Storybook / múltiplos remotes MF / mobile. *(Nota: MF já implementado no Tier-0 como fora-de-sequência.)*

### Track D — Database (DB0-4)
**🟢 DB0 — schema SQL + lore** — checkpoint: *"banco local com Cat_CognicaoAparente, Cat_Periculosidade, Anomalia, SPs"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `cognicao-tabela` | `Cat_CognicaoAparente` existe com SE/SA/IN/AA | ref/18, decco.sql |
| `periculosidade-tabela` | `Cat_Periculosidade` existe com 9 níveis | ref/18 |
| `anomalia-oa` | `Anomalia` tem `CognicaoAparenteId` e `PericulosidadeId` | ref/18 |
| `sps-novas` | SPs `sp_Laboratorio_Inserir`, `sp_ProtocoloContencao_Inserir`, `sp_NotificacaoAnomalia_Inserir` | ref/18 |

**🟡 DB1 — Docker + auth tables** — checkpoint: *"SQL Server em container Docker + tabelas de autenticação"*.
| id | Sinal decisivo | Doc |
|---|---|---|
| `docker-sql` | `docker-compose.yml` com serviço SQL Server + `docker ps` mostra container rodando | knowledge-drops/024 |
| `user-table` | Tabela/entidade `User` com `Username`, `PasswordHash`, `Clearance`, `SiteId` | ref/13, ref/18 |
| `role-table` | Tabela/entidade `Role` com `Name` | ref/13 |
| `usersite-table` | Tabela/entidade `UserSite` relacionando User ↔ Site | ref/13 |

**🔵 DB2 — CRUDs de catálogo:** endpoints + telas para Cat_CognicaoAparente, Cat_Periculosidade, Laboratorio, ProtocoloContencao, NotificacaoAnomalia.
**🟣 DB3 — NoSQL/busca:** MongoDB/Couchbase (documento agregado) + Elasticsearch (índices + CDC).
**⚫ DB4 — Poliglota:** Redis (FusionCache) + Kafka (eventos de domínio) + Vault (segredos).

### Track E — Micro-frontend (track emergente)
| id | Sinal decisivo | Doc |
|---|---|---|
| `mf-host` | `vite.config.ts` com `@module-federation/enhanced` plugin + `remotes` config | knowledge-drops/024 |
| `mf-remote` | `vite.config.ts` com `@module-federation/vite` plugin + `exposes` | knowledge-drops/024 |
| `mf-loadRemote` | `MicroFrontendModal.tsx` com `loadRemote('decco-demo/./DashboardWidget')` | knowledge-drops/024 |
| `mf-static-files` | Back-end serve `mf-remote/dist` via `UseStaticFiles` | knowledge-drops/024 |

> Track E é **emergente** — Module Federation foi implementado precocemente (conceitualmente FE4) como exercício de aprendizado.
> O diagnóstico deve reportá-lo como *"fora-de-sequência legítimo (previsto)"*.

### Vertical Auth (Tier 1, obrigatório no v0.2.0)
| fase | id | Sinal decisivo | Doc |
|---|---|---|---|
| 1.0 docker | `docker-sql` | `docker-compose.yml` com SQL Server + container rodando | ref/12 |
| 1.1 token/hash | `auth-token` | `AddJwtBearer` + `POST /api/Auth/token` + `PasswordHasher`/PBKDF2. **Anti-sinal**: `AddIdentity`/`IdentityDbContext` (fora do modelo Omnibees) | ref/12, ref/13 |
| 1.2 permissão | `auth-apipermission` | `ApiPermission` (EF code-first) + seed + deny-by-default | ref/12 |
| 1.3 recurso | `auth-clearance-sitio` | `User`/`Role`/`UserSite` + `Clearance`; filtro-de-query (List) + `AuthorizationHandler<ClearanceRequirement,Anomalia>` | ref/13 |

## 3. Derivação do "Tier observado"
- **Tier fechado:** o **maior tier cujos conceitos-checkpoint (obrigatórios) estão TODOS presentes**, exigindo que **todos os tiers
  anteriores** também estejam completos (sequência cumulativa).
- **Tier parcial:** se o tier corrente tem alguns mas não todos → `Tier N em andamento (x/y)`, listando **presentes** e **faltantes**.
- **Opcionais** (`foundation-api`, `multi-scope`) **não entram** no denominador `y`; reportar à parte, nunca bloqueiam.
- **Por track, independente.** Ex.: *"Front: FE0 completo (7/7) · Back: T0 completo (7/7) · DB: DB0 completo (4/4) · Auth: não iniciado"*.
- **Track E (micro-frontend)** não tem progressão de tier — ou existe ou não. Se existe, reportar como "MF presente (fora-de-sequência)".

## 4. Fora-de-sequência (aviso didático, não-bloqueante)
Ocorre quando o código tem um conceito de **tier superior** enquanto o **tier corrente ainda não fechou**. É **aviso, não erro** —
os tiers são **guia, não cerca** (o sandbox é livre; puxar um vertical para cedo é legítimo).

### Fora-de-sequência conhecido: Module Federation no Tier-0
O micro-frontend (conceitualmente FE4 / Tier-4) foi implementado **já no Tier-0**. Isto é **fora-de-sequência intencional** e
deve ser reportado como:
```
ℹ️ Fora-de-sequência (legítimo / previsto)
   Detectado: Module Federation (Track E — pertence ao Tier 4 conceitual)
   Tier corrente: FE0 (modelo-template)
   Por quê: a equipe optou por aprender MF cedo como experimento didático.
   Status: funcional (host + remote + loadRemote).
   O que falta do MF Tier-4 real: deploy independente, múltiplos remotes, Storybook.
```

### Regras gerais
- Um aviso por conceito adiantado; **nunca** linguagem de reprovação ("errado/proibido") — usar "adiantado/fora da ordem sugerida".
- Sempre acompanhar do caminho para fechar o tier corrente.
- O opt-in **explícito** (`auth-rica` após T0 completo) **não** gera aviso — é rota prevista ("vertical adiantado (previsto)").
- Module Federation no Tier-0 é considerado **fora-de-sequência legítimo** e tem mensagem especial (acima).

### Outros exemplos de fora-de-sequência
- `Redis`/`FusionCache` instalado mas sem auth → "cache presente sem autenticação (Tier 2 adiantado)"
- `Kafka`/`MassTransit` instalado mas sem CRUD básico → "mensageria presente sem entidades base (Tier 2 adiantado)"
- `docker-compose.yml` com Redis + Kafka mas sem SQL Server + auth → "orquestração presente sem banco de auth (Tier 1 incompleto)"

## 5. Reconciliação DECLARADO × OBSERVADO
**Regra mestra: o OBSERVADO (código) tem precedência.** O declarado (`DECCO-PROGRESS.md`) nunca altera o veredito de tier — só
gera **avisos de divergência**. Se o arquivo não existir: reportar só o observado e **oferecer** criá-lo (opcional).
| Declarado | Observado | Veredito | Ação |
|---|---|---|---|
| ✅ feito | ✅ presente | Confirmado | contabiliza no tier |
| ✅ feito | ❌ ausente | ⚠️ Declarado-sem-observação | aviso; vale o observado (não conta) |
| ❌ não-feito | ✅ presente | ℹ️ Observado-não-declarado | aviso ameno; sugerir atualizar o arquivo; conta |
| ❌ não-feito | ❌ ausente | Coerente (pendente) | nenhuma |

## 6. Formato do relatório de diagnóstico — novo template (Tier-0 real)

```
🛰️  DIAGNÓSTICO DECCO — <data> — <repo/caminho>
Tracks detectados: A (moderno) · C (front) · D (DB) · E (micro-frontend)

▸ Track A — Back moderno ..... Tier 0 (modelo-template) COMPLETO (7/7)
   Presentes: envelope, dbcontext-efcore, repo-ef, repo-dapper-sp, host-minimal, serve-mf-remote, git-tag-template
   Próximo: Tier 1 (login real) — docker-compose, auth-jwt, auth-endpoint, password-hasher, user-table

▸ Track C — Front ............ FE0 (modelo-template) COMPLETO (7/7)
   Presentes: seam-dados, login-mock, micro-frontend, dashboard, estados-4, modais-radix, tier-content
   Próximo: FE1 (login real) — login-real, jwt-context, auth-fetch, logout

▸ Track D — DB ............... DB0 COMPLETO (4/4)
   Presentes: cognicao-tabela, periculosidade-tabela, anomalia-oa, sps-novas
   Próximo: DB1 (Docker + auth) — docker-sql, user-table, role-table, usersite-table

▸ Track E — Micro-frontend ... PRESENTE (fora-de-sequência legítimo)
   MF funcional: host + remote + loadRemote + static-files
   O que falta p/ MF Tier-4: deploy independente, múltiplos remotes, Storybook

Avisos:
   ℹ️ Fora-de-sequência (legítimo): Module Federation implementado no Tier-0 (conceitual FE4).
   ℹ️ Nenhuma divergência declarado×observado.

Resumo: projeto está no modelo-template (Tier 0). Próximo checkpoint: v0.2.0 com login real.

🔎 Fio a puxar: implemente docker-compose com SQL Server + POST /api/Auth/token + tela de login real.
   É o exercício integrador que cruza DB → BE → FE → GT simultaneamente.
```

## 7. Fio a puxar
Compare o diagnóstico **antes** de começar o Tier-1 (modelo-template puro) com o diagnóstico **depois** de implementar login
real. O contraste `Back: T0 (7/7) + Auth: não iniciado` → `Back: T1 completo (7/7)` é o feedback-loop do aprendizado.
Quando o Tier-1 fechar, o diagnóstico deve mostrar que Docker + JWT + login real + permissionamento estão todos verdes.
