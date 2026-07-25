# 12 — Autenticação e permissionamento: o modelo Omnibees e como espelhar no Decco

> Dúvida do Paulo: o banco da OB.API tem **tabelas de permissionamento** que o DeccoDB **não** tem — como proceder? São
> geradas **manualmente** ou **junto com o ASP.NET Identity**? Resposta dissecada dos repos reais (Partners + OB.API). **É aferível.**
> Regra de foco: **auth é Tier 2**, não Tier 0 (ver `reference/11`).
>
> **Para a versão RICA** (papéis + permissões + autorização a nível de recurso — "nem todos veem todas as anomalias" via
> clearance-level + sítio) e o **build passo-a-passo**, ver `reference/13` (design) + `recipes/02-auth-vertical.md` (recipe). Este
> `reference/12` é o **modelo Omnibees + as opções**; o `13` é o **como construir a camada rica**.

## 1. O modelo Omnibees (verificado) — 2 camadas, SEM ASP.NET Identity

**⛔ ASP.NET Identity NÃO é usado em lugar nenhum** (nem Partners, nem OB.API). Grep confirmou ausência total de
`AddIdentity`/`IdentityDbContext`/`IdentityUser`/`UserManager<`/`AspNetUsers`/`AspNetRoles`. → **as tabelas de permissão NÃO são
geradas pelo Identity.** Nem existe "gerar junto com Identity" no mundo Omnibees.

### Camada 1 — Autenticação (quem é): token EXTERNO
- **JWT validado contra um IdentityServer/AuthServer externo** (`Authority`). O serviço é **resource server** — só **valida** o
  token; **não** emite token de usuário, **não** tem login/user-store local.
- Partners: `AddJwtBearer(options.Authority = ...)` (`Program.cs:119-133`). OB.API: OWIN OAuth Bearer (host interno) /
  `IdentityServer3.AccessTokenValidation` (host público). Identidade vem dos **claims** (`obUserId`, `wl_uid`, `EsId`…).
- (Curiosidade OB.API: há **um** token gerado localmente — `_adminToken` de impersonação Admin por **subnet confiável** — caso de borda, não é auth de usuário final.)

### Camada 2 — Autorização (o que pode): tabelas próprias + deny-by-default
Há **DOIS stores de permissão distintos**, em DOIS lugares:

| Store | Onde vive | O que guarda | Como é criado |
|---|---|---|---|
| **Core (OB.API)** | banco **Omnibees** (SQL Server, EDMX `General.edmx`) | `Users`, `Roles`, `RolesPermissions`, `UsersRoles`, `UserProperties`, `UsersWhiteLabelClients`, `ApiPermissions` (+ `BackofficeMenu`, `UserIps`, `UserFingerprints`…) | **database-first, feito à mão** (schema proprietário; SP `GetInternalUserPermissions` agrega tudo num `CustomPrincipal.Permissions` = `Dictionary<PermissionCode, operações>`, cacheado no Couchbase) |
| **Conector (Partners)** | banco próprio do conector (`PartnersApiDbContext`, EF Core) — 2 tabelas: **`ApiPermission`** + `EsCredential` | permissão **por endpoint** (`ActionName`, `Group`, `PermissionCode`, `IsRead/IsAdd/IsUpdate/IsDelete/IsExecute`) | **EF Core code-first**: entidade C# scaffolded → `EnsureCreated()` (só em Development) cria a tabela → **seed via `HasData`** |

- O catálogo de códigos de permissão é um **enum** `PermissionCodes` (em `OB.BL.Constants`), **não** uma tabela-mestre.
- **Decisão de autorização** (`AuthService.UserIsAuthorizedToCallMethod`): cruza (a) o que o **endpoint exige** (`ApiPermissions`, mesmo `ActionName`+`Group` = AND) com (b) o que o **user possui** (vindo da OB.API `POST /api/User/GetApplicationUserInfo`, cacheado). **Fail-safe: nega por defeito** (endpoint sem linha → 403). Recusa: V1→401, V2→403.

### Resposta direta à dúvida
> **Nem "manual" nem "Identity" de forma simples:** no **core** as tabelas são **hand-made / database-first** (parte do schema
> Omnibees, criadas por SQL, mapeadas por EDMX). No **conector** a `ApiPermission` é **EF Core code-first** — você define a classe
> e o **EF cria a tabela** (`EnsureCreated`/migração) + **semeia com `HasData`**. **Identity não entra em nada disso.**

## 2. Como espelhar no Decco (Tier 2 — documentado, NÃO no Tier 0)

**Tier 0 = SEM auth.** O slice da Anomalia roda com `[AllowAnonymous]` (ou sem auth wired). Não bloquear o vertical slice em auth.

Quando auth entrar (Tier 2), espelhar as 2 camadas. Três sub-decisões:

### (a) Autenticação — token externo (não Identity)
- **Dev/sandbox (recomendado):** um **emissor JWT de dev** minúsculo (self-signed) OU o `dotnet user-jwts` — a API valida via `AddJwtBearer` (resource server), fiel à Omnibees. 
- **Fiel-pesado:** subir **Duende IdentityServer** ou **Keycloak** local.
- **ASP.NET Identity** é uma abordagem **diferente** (user-store local + `AspNetUsers`) — vale **estudar como contraste**, mas **não** é o que a Omnibees faz; não confundir "mirror" com "Identity".

### (b) As tabelas que o DeccoDB não tem — 3 opções de criação
1. **EF Core code-first (recomendado para a `ApiPermissions` do conector):** define a entidade C#, `EnsureCreated()`/migração cria, `HasData` semeia. **Espelha o Partners exatamente**; zero DDL à mão.
2. **Script SQL manual (`decco-auth.sql`) no core:** para os análogos de `Users`/`Roles`/`RolesPermissions` (as tabelas que no OB.API são database-first), escrever o DDL + seed e rodar junto com o `decco.sql`. Fiel à realidade (lá são hand-made) e treina database-first.
3. **Banco/contexto de auth separado:** manter as tabelas de auth **fora** do `DeccoDB` de domínio (ex.: contexto próprio do conector, ou `DeccoAuthDB`). **Mais limpo** e espelha a realidade — o Partners guarda `ApiPermission` no **seu** banco, não no banco de domínio da OB.

### (c) Onde cada peça mora (espelhando a realidade)
- **`ApiPermissions` (por endpoint)** → no **conector (Foundation.API)**, no **próprio banco/contexto dele** (EF code-first + `HasData`). ⇐ igual ao Partners.
- **`Users`/`Roles`/`RolesPermissions`/`UsersRoles` (quem tem o quê)** → no **core (Decco.API)** / `DeccoDB` (ou `DeccoAuthDB`), via `decco-auth.sql`, com uma **SP `GetUserPermissions`** que agrega → o core expõe um `GetApplicationUserInfo`-like que o conector consome. ⇐ igual ao OB.API.
- **Enum `PermissionCodes`** em código (não tabela).
- **Recomendação:** conector = EF code-first (`ApiPermissions`) ; core = `decco-auth.sql` (Users/Roles/Perms) + SP ; autenticação = JWT de dev. Tudo **Tier 2**.

## Porquê / fio a puxar
**Porquê separar os dois stores:** o "o que o endpoint exige" (conector) muda com a API; "o que o user tem" (core) muda com o
negócio — acoplá-los num só lugar mistura dois ciclos de vida. *Alternativa rejeitada:* jogar tudo numa tabela só no DeccoDB —
some a lição da separação core↔conector.
🔎 **Fio:** faça primeiro a `ApiPermissions` do conector por **EF code-first + HasData** (rápido, 1 tabela) e valide o
**deny-by-default** com um endpoint protegido e um JWT de dev — antes de modelar Users/Roles no core. Registre a escolha de
"banco separado vs no DeccoDB" quando chegar lá (`open-questions/`).
