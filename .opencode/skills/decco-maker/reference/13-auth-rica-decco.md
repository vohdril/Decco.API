# 13 — Auth RICA no Decco: dados sensíveis + papéis/permissões + autorização a nível de recurso

> Complementa `reference/12` (o modelo Omnibees + as opções). Aqui está o **design concreto e autossuficiente** para CONSTRUIR
> uma camada de auth no Decco **mais rica** que a Omnibees: papéis, permissões, e **"nem todos veem todas as anomalias"** via
> **clearance-level** + **sítio de contenção** (à la SCP/MIB). Escrito para ser gerável por qualquer modelo/IA a partir daqui.
> Provenance opcional (não é dependência): repos em `C:\Git`, fontes em `omni-src`. **Nada de segredo em texto; sem dados de cartão (PCI).**

## 1. O modelo Omnibees (verificado — a base a espelhar)
- **Autenticação:** token **externo** (AuthServer/IdentityServer). Os serviços são *resource servers* (só validam JWT). A senha é **apenas gravada com hash** (`ISecurityProvider.GenerateSalt()`+`HashPassword()` → binário `OB.Security`; ⚠️ **salt = GUID**, fraco — **não copiar**); a **validação de login vive no AuthServer**, fora do código.
- **Autorização = cascata de 3 degraus** (`CustomPrincipal`): (1) `UnlimitedAccess` (interno) → passa; (2) tem o **PermissionCode de bypass** (`8604` AllClients, `8605` AllProperties, `8611` AllWhiteLabels) → passa; (3) senão, o recurso pedido tem de estar no **conjunto de escopo** (`ClientIds`/`PropertyIds`/`WhiteLabelIds`).
- **Papéis→permissões:** `Roles`+`RolesPermissions`+`UsersRoles` colapsam num `Dictionary<PermissionCode,{Read,Add,Update,Delete,Execute}>` pela SP `GetInternalUserPermissions` (`GroupBy(PermissionCode)` = OR das flags entre papéis), cacheado.
- **Imposição** (não centralizada; por operação): **(A) filtro-de-query** (recorta os ids ao escopo; vazio→lista vazia) e **(B) check por-item** (valida o id antes de gravar/devolver; `Unauthorized`). Default **fail-closed**. Partners usa quase só (B), via `ValidateHelper.ValidateHotelAccess` no topo de cada método.
- **Dados sensíveis:** segredos cifrados via **Vault Transit** (`EsCredential.Credentials` é cifrado **antes** de persistir); cartão via **Encrypt.API** externa (colunas `CreditCard*` em `Users` são só contadores/quota, não PAN); leitura de cartão gateada por PermissionCode.

## 2. Como o Decco ENRIQUECE (o pedido do Paulo)
Mesma cascata de 3 degraus, instanciada com **duas dimensões de escopo** que já existem no DeccoDB:

| Dimensão | Campo natural no DeccoDB | Regra |
|---|---|---|
| **Clearance level** (hierárquico) | `Cat_ClasseObjeto.NivelAcessoMinimo` (1=PACATO … 4=UKAR) | user vê a Anomalia se `user.Clearance >= anomalia.Classe.NivelAcessoMinimo` |
| **Sítio de contenção** (conjunto) | `Anomalia.SitioContencao` | user vê se `AccessToAllSites` **ou** `anomalia.SitioContencao ∈ user.Sites` |

Cascata final (idêntica à Omnibees, mais rica): **(1)** role `O5/Administrador` (`Unlimited`) → tudo; **(2)** bypass `AccessToAllSites` / `AccessToAllClearances` → passa aquela dimensão; **(3)** `Clearance >= classe.min` **E** `site ∈ user.Sites`. Mais os **papéis** (Pesquisador=Read, Agente de Contenção=Read+Update, Diretor de Sítio=CRUD no seu sítio, O5=Unlimited) mapeados a **PermissionCodes** (por operação CRUD-E), como no Omnibees.

## 3. Dados sensíveis NO DECCO (seguro; e a comparação com Identity que você conhece)
Diferença-chave: **o Decco NÃO tem AuthServer externo** → a validação de senha e a emissão de token **passam a ser SUAS** (você constrói o pedaço que a Omnibees terceiriza). Bom para estudo.
- **Hash de senha:** usar o **`PasswordHasher<T>` do ASP.NET Core Identity** (PBKDF2, salt por **RNG**, iterações, formato versionado) — **sem** adotar o Identity inteiro; ou `Rfc2898DeriveBytes` (PBKDF2) próprio; ou BCrypt. ⚠️ **Não** replicar o `salt = Guid` do Omnibees (é fraco). *Comparação:* é exatamente o que o Identity faz internamente no login — a diferença é que a Omnibees delega isso ao AuthServer; aqui você faz local.
- **Lockout / 2FA:** modelar `IsLocked`, `LoginAttempts`, `IsTwoFactorEnabled` no `User` (como o `Users` da Omnibees tem) — estudo opcional de segurança de conta.
- **Segredos / cifra-at-rest:** análogo .NET do Vault Transit = **`IDataProtector`** (ASP.NET Core Data Protection) para cifrar campos sensíveis antes de persistir; segredos de config via **user-secrets/env vars** (nunca em texto). Registar que em produção seria **Vault Transit** (como o `EsCredential` da Partners). Abstrair atrás de um `ISecretProtector`/`ICryptoProvider` (troca dev↔Vault).
- **PCI:** o domínio Decco é fictício e **não tem cartão** — **não modelar PAN/CVV**. Se algum estudo pedir "pagamento", usar dado fictício não-cartão. (Boundary PCI mantido.)

## 4. ONDE vive cada peça (espelha a realidade e enriquece)
- **Core (Decco.API)** = fonte de verdade de identidade/escopo: tabelas `User`, `Role`, `Permission`(/`RolePermission`), `UserRole`, `UserSite` + `Clearance` no user; uma SP `sp_User_GetPermissions` (agrega papéis→permissões, como a `GetInternalUserPermissions`). **E o emissor de token** (o pedaço que a Omnibees terceiriza ao AuthServer): um endpoint `POST /api/Auth/Token` que valida a senha (hash) e emite um **JWT** com claims `sub`, `roles`, `clearance`, `sites` (ou um `scope`).
- **Conector (Foundation.API)** = borda pública: valida o JWT (`AddJwtBearer`), tem a sua **`ApiPermissions`** (EF code-first + `HasData`, por endpoint) e faz a **imposição** no envelope.
- **Enum `PermissionCodes`** em código (não tabela), como na Omnibees.

## 5. Imposição no Decco (padrões A + B — precisa dos dois)
- **List → Padrão A (filtro-de-query):** a query de anomalias é recortada a `clearance >= classe.min` **e** `site ∈ user.Sites` (a menos de bypass) → o utilizador só **recebe** o que pode ver (fail-closed = lista vazia).
- **Get/Update/Delete → Padrão B (check por-item):** um `ValidateHelper.ValidateAnomaliaAccess(anomalia, user)` no **topo** da operação (como o `ValidateHotelAccess` do Partners); se não passa a cascata → `Unauthorized` (403 no V2). Fail-closed.
- O check por-endpoint (o que a operação exige) continua vindo da `ApiPermissions` (deny-by-default), como em `reference/12`.

## 6. Blocos .NET nativos (para gerar sem depender de nada externo)
- **Autenticação:** `builder.Services.AddAuthentication(JwtBearerDefaults...).AddJwtBearer(o => { o.TokenValidationParameters = new(){ ValidateIssuer/Audience/Lifetime, IssuerSigningKey = <symmetric dev key de config> }; })`.
- **Emissão de token:** `JwtSecurityTokenHandler` (ou `Microsoft.IdentityModel.JsonWebTokens`) assinando com `SymmetricSecurityKey` (chave de dev via user-secrets); claims custom `clearance`, `site` (múltiplos), `role`.
- **Senha:** `new PasswordHasher<User>().HashPassword(user, pwd)` / `.VerifyHashedPassword(...)`.
- **Autorização por papel/permissão:** política (`AddAuthorization(o => o.AddPolicy("CanReadAnomalia", p => p.RequireAssertion(...))`) **ou** o filtro custom estilo Omnibees (`ApiPermissions` + `[Authorize]`).
- **Autorização a nível de recurso (o coração):** **resource-based authorization** do ASP.NET Core — `IAuthorizationRequirement` (`ClearanceRequirement`) + `AuthorizationHandler<ClearanceRequirement, Anomalia>` que checa `user.Clearance >= anomalia.Classe.NivelAcessoMinimo && (bypassAllSites || user.Sites.Contains(anomalia.SitioContencao))`, invocado com `IAuthorizationService.AuthorizeAsync(User, anomalia, new ClearanceRequirement())`. É o idioma .NET para "este user pode acessar ESTA anomalia".

## 7. Foco / tier
Isto é um **vertical de auth dedicado** (opt-in), a fazer **depois** do primeiro slice da Anomalia (Tier 0) — para haver o que proteger. Não é Tier 0. A ordem concreta de construção está em `recipes/02-auth-vertical.md`.

## Porquê / fio a puxar
**Porquê clearance (hierárquico) + sítio (conjunto)** e não só uma lista de ids: modela os **dois** eixos reais de acesso (nível de segredo × unidade organizacional) — é o mapeamento fiel de `PermissionCode-bypass` (nível) + `PropertyIds` (conjunto) da Omnibees, com semântica de domínio. 🔎 **Fio:** compare **resource-based authorization** (handler por recurso) vs **query-filter** (recorte na leitura) — quando cada um falha sozinho? (resposta: List precisa de filtro; Get/Update precisam de handler — por isso os dois).
