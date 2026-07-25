# 007 — Auth/permissionamento: NÃO é ASP.NET Identity; plano de espelho (Tier 2)

- **Data de registo:** 2026-07-13
- **Fonte:** deep-dive read-only em `partners-api` + `bhi-ob-api`/`omni-src\OB.API` (dúvida do Paulo sobre as tabelas de permissão)
- **Tipo:** conhecimento resolvido + decisão de rumo
- **Afeta:** `reference/12` (novo), `reference/11` (auth = Tier 2), escopo do Tier 0

## Resposta à dúvida (conclusiva)
As tabelas de permissionamento do mundo Omnibees **NÃO são geradas por ASP.NET Identity** — **Identity não é usado em lado nenhum**
(sem `AddIdentity`/`IdentityDbContext`/`AspNetUsers`/`UserManager<`). Há **dois stores distintos**:
- **Core (OB.API):** tabelas **hand-made / database-first** no banco Omnibees (`Users`, `Roles`, `RolesPermissions`, `UsersRoles`, `UserProperties`, `UsersWhiteLabelClients`, `ApiPermissions`), agregadas pela SP `GetInternalUserPermissions` → `CustomPrincipal.Permissions`.
- **Conector (Partners):** a `ApiPermission` (por endpoint) é **EF Core code-first** — entidade C# → `EnsureCreated()` (só Dev) cria a tabela → **seed via `HasData`**. (2º DbSet: `EsCredential`.) **Não há Migrations**; em prod a tabela pré-existe.
- **Autenticação:** token **externo** (IdentityServer/AuthServer); os serviços são **resource servers** (só validam JWT), sem user-store local. (OB.API tem só um `_adminToken` de impersonação por subnet — caso de borda.)

## Decisão para o Decco
- **Auth é Tier 2, não Tier 0.** O slice da Anomalia (Tier 0) roda **sem auth** (`[AllowAnonymous]`). Não bloquear o vertical slice.
- **Quando entrar (Tier 2), espelhar as 2 camadas** (detalhe e opções em `reference/12`):
  - **Autenticação:** JWT de dev (self-signed / `dotnet user-jwts`) validado via `AddJwtBearer` — fiel ao "resource server". Duende/Keycloak se quiser fiel-pesado. **Não** usar ASP.NET Identity para "espelhar" (é abordagem diferente; serve como contraste de estudo).
  - **Autorização:** `ApiPermissions` (por endpoint) no **conector**, **EF code-first + `HasData`** (espelha o Partners, 1 tabela, rápido); `Users`/`Roles`/`RolesPermissions` no **core**, via **`decco-auth.sql`** + SP `GetUserPermissions` (espelha o OB.API database-first). Enum `PermissionCodes` em código.
- **As tabelas que o DeccoDB não tem** criam-se por: (1) **EF code-first** (recomendado p/ a do conector) ; (2) **script SQL** `decco-auth.sql` (p/ as do core) ; (3) **banco/contexto de auth separado** (mais limpo, espelha a realidade — o Partners guarda a `ApiPermission` no banco DELE, não no de domínio).

## Como aplicar
- Não gerar auth no Tier 0. Ao chegar no Tier 2, começar pela `ApiPermissions` do conector (EF code-first + `HasData` + `[Authorize]` + JWT de dev + **deny-by-default**), validar, e só então modelar Users/Roles no core.
- Se o Paulo perguntar "gero manual ou com Identity?": **não é Identity**; é **EF code-first (conector)** e/ou **SQL database-first (core)** — ver `reference/12`.

## Fio a puxar
Onde colocar as tabelas de auth: **DeccoDB** (junto do domínio) vs **DeccoAuthDB/contexto separado** (mais limpo, fiel ao Partners)? Decidir ao entrar no Tier 2 (candidato a `open-questions/`).
