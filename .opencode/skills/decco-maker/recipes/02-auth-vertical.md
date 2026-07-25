# recipes/02 — Vertical de Autenticação + Autorização (rica) do Decco

> Molde **acionável e autossuficiente** para construir a camada de auth do Decco (papéis + permissões + **clearance-level** +
> **sítio de contenção**), espelhando a Omnibees e enriquecendo. Design e porquês em `reference/13`; modelo Omnibees em `reference/12`.
> **Pré-requisito:** o slice da Anomalia (Tier 0) já existir (é preciso ter o que proteger). **Regras:** nada de segredo em texto
> (usar user-secrets/env vars); **sem dados de cartão**; dados fictícios (agentes `Agente-Alpha`, sítios `Sitio-19`, `Sitio-64`).

## Ordem de construção (com checkpoints)

### Etapa A — Esquema de identidade no CORE (Decco.API)
Tabelas (via `decco-auth.sql` **ou** EF code-first — ver `reference/12` §2b). Mínimo:
- `User` (`Id`, `Username`, `PasswordHash`, `Clearance INT` 1..4, `IsActive`, `IsLocked`, `LoginAttempts`, `IsTwoFactorEnabled`) — **sem** coluna de senha em claro; **sem** cartão.
- `Role` (`Id`, `Codigo` ex.: `PESQUISADOR|AGENTE_CONTENCAO|DIRETOR_SITIO|O5`, `Nome`), `Permission` (`Id`, `PermissionCode INT`, `Nome`), `RolePermission` (`RoleId`, `PermissionId`, flags `CanRead/CanAdd/CanUpdate/CanDelete/CanExecute`), `UserRole` (`UserId`, `RoleId`), `UserSite` (`UserId`, `SitioContencao`).
- SP `sp_User_GetPermissions(@UserId)` → result-sets: info do user (Clearance/flags), permissões agregadas (join UserRole→RolePermission, `GroupBy(PermissionCode)` = OR das flags), sítios do user. (Espelha `GetInternalUserPermissions`.)
- Seed fictício: `O5` (Unlimited + AccessToAllSites + Clearance 4), `Diretor de Sitio-19` (Clearance 3, sites {Sitio-19}, CRUD), `Pesquisador` (Clearance 2, sites {Sitio-19}, Read).
✅ **Verificação:** `EXEC sp_User_GetPermissions` devolve o dicionário certo para cada seed.

### Etapa B — Emissor de token no CORE (o que a Omnibees terceiriza ao AuthServer)
`POST /api/Auth/Token` (envelope): valida `Username`+senha e emite JWT.
```csharp
// hash na escrita / verificação no login — PasswordHasher do Identity (PBKDF2 + salt RNG); NUNCA salt=Guid
var hasher = new PasswordHasher<User>();
// criar:   user.PasswordHash = hasher.HashPassword(user, plainPwd);
// validar: hasher.VerifyHashedPassword(user, user.PasswordHash, plainPwd) == PasswordHashResult.Success
// emitir JWT (chave simétrica de DEV vinda de config/user-secrets — // >>> nunca hardcodar a chave)
var claims = new List<Claim> {
    new("sub", user.Id.ToString()),
    new("clearance", user.Clearance.ToString()),
    // um claim "site" por sítio + os roles/permissionCodes que o user tem
};
// user.Sites.ForEach(s => claims.Add(new Claim("site", s)));
var key = new SymmetricSecurityKey(Convert.FromBase64String(cfg["Auth:DevSigningKey"]!)); // >>> chave só em user-secrets
var jwt = new JwtSecurityToken(issuer: "decco", audience: "decco", claims: claims,
              expires: /* now+X */ default, signingCredentials: new(key, SecurityAlgorithms.HmacSha256));
// return new JwtSecurityTokenHandler().WriteToken(jwt);
```
✅ **Verificação:** login com senha certa devolve JWT com claims `sub/clearance/site/role`; senha errada → `Unauthorized` (e incrementa `LoginAttempts`).

### Etapa C — Validação do token na FACHADA (Foundation.API) + ApiPermissions
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    o.TokenValidationParameters = new TokenValidationParameters {
        ValidIssuer = "decco", ValidAudience = "decco", ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(cfg["Auth:DevSigningKey"]!)) });
```
- `ApiPermission` (EF code-first + `HasData`) por endpoint (ver `reference/12`); filtro de autorização por endpoint com **deny-by-default** (V2→403). O `AuthUser` (imutável) carrega `Clearance`, `Sites`, `Permissions` (dict PermissionCode→ops), reidratado das claims (ou de um `GetUserInfo` ao core).
✅ **Verificação:** endpoint sem token → 401; token válido sem a permissão do endpoint → 403.

### Etapa D — Autorização a nível de RECURSO (o "nem todos veem todas as anomalias")
**Padrão B (Get/Update/Delete) — resource-based authorization:**
```csharp
public sealed class ClearanceRequirement : IAuthorizationRequirement { }

public sealed class AnomaliaAuthorizationHandler : AuthorizationHandler<ClearanceRequirement, Anomalia>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext ctx, ClearanceRequirement req, Anomalia anomalia)
    {
        var clearance = int.Parse(ctx.User.FindFirst("clearance")?.Value ?? "0");
        var sites = ctx.User.FindAll("site").Select(c => c.Value).ToHashSet();
        var unlimited = ctx.User.IsInRole("O5");
        var bypassSites = /* tem PermissionCode AccessToAllSites */ false;
        var ok = unlimited || (clearance >= anomalia.ClasseNivelAcessoMinimo
                               && (bypassSites || sites.Contains(anomalia.SitioContencao)));
        if (ok) ctx.Succeed(req);
        return Task.CompletedTask;
    }
}
// uso no serviço (topo da operação, como o ValidateHotelAccess do Partners):
// var r = await _authz.AuthorizeAsync(User, anomalia, new ClearanceRequirement());
// if (!r.Succeeded) => response com ErrorCodes.Unauthorized (fail-closed)
```
**Padrão A (List) — filtro-de-query:** recortar as anomalias por `Clearance >= Classe.NivelAcessoMinimo && (bypass || SitioContencao ∈ user.Sites)` **antes** de devolver (fail-closed = lista vazia). No core, passar `clearance`+`sites` como parâmetros da busca (ex.: um `@ClearanceMax`/TVP de sítios na `sp_Anomalia_Buscar`, ou um `Where` no EF).
✅ **Verificação:** um Pesquisador (Clearance 2, Sitio-19) **lista** só anomalias de classe ≤2 no Sitio-19; **Get** de uma anomalia UKAR (nível 4) → 403; o O5 vê tudo.

### Etapa E — Dados sensíveis (transversal)
- Cifra-at-rest de campos sensíveis (se houver): `IDataProtector` (análogo .NET do Vault Transit) atrás de um `ISecretProtector` (dev = DataProtection; prod = Vault). **Nunca** persistir em claro; **nunca** logar segredo/hash.
- Chave de assinatura JWT e connection string: **user-secrets/env vars** (`// >>> nunca em appsettings versionado`).
- **Sem cartão** (PCI): não modelar PAN/CVV.

## Marcadores e assunções
- `// >>>` nos pontos que dependem de decisão (chave de dev, se `ApiPermissions` fica no banco do conector vs separado — `open-questions/Q-007`; algoritmo de hash se não for o PasswordHasher).
- Assumir por convenção: JWT HS256 de dev; PermissionCodes num enum; cascata de 3 degraus de `reference/13`.

## Fecho
Ao gerar, entregar por etapa (A→E), cada uma com o seu checkpoint, **um tier/vertical por vez** (não despejar tudo). Terminar
listando os `// >>>` e **um fio a puxar** (ex.: "trocar o emissor de token dev por Duende IdentityServer — o que muda no cliente?").
