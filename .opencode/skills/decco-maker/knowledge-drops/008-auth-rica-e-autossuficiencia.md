# 008 — Camada de auth RICA no Decco + refinamento de autossuficiência

- **Data de registo:** 2026-07-13
- **Fonte:** deep-dive read-only (dados sensíveis + autorização a nível de recurso em Partners/OB.API) + decisão do Paulo de CONSTRUIR uma auth rica e de a skill ser autossuficiente para outros modelos/IA
- **Tipo:** conhecimento resolvido (dados sensíveis) + decisão de rumo (auth como vertical de 1ª classe; autossuficiência)
- **Afeta:** `reference/13` (novo), `recipes/02` (novo), `reference/11/12`, SKILL.md
- **Complementa:** `knowledge-drops/007` (auth não é Identity; token externo; 2 stores)

## A — Dados sensíveis no Omnibees (verificado)
- **Senha:** só **gravada** com hash (`ISecurityProvider`+`HashPassword` → binário `OB.Security`; ⚠️ **salt = GUID**, fraco). A **validação de login é EXTERNA (AuthServer)** — não está nos repos. `IsLocked`/`LoginAttempts`/`IsTwoFactorEnabled` existem em `Users`, consumidos pelo AuthServer.
- **Segredos:** cifrados via **Vault Transit** — o `EsCredential.Credentials` é cifrado **antes** de persistir (`ConfigurationService` → `_vault.EncryptAsync`). Legado: `HashicorpVaultRepository` (batch).
- **Cartão (PCI):** via **Encrypt.API** externa; colunas `CreditCard*` em `Users` são **contadores/quota**, não PAN; leitura gateada por PermissionCode (`CanViewCreditCardDetails`, code 5026). **Não modelar cartão no Decco.**

## B — Autorização a nível de recurso (verificado) — a base do "nem todos veem todas as anomalias"
- Escopo no principal: `ClientIds`/`PropertyIds`/`WhiteLabelIds` (conjuntos) + `Permissions` (dict PermissionCode→ops) + `UnlimitedAccess`.
- **Cascata de 3 degraus:** (1) interno/unlimited → passa; (2) PermissionCode de **bypass** (`8604/8605/8611`) → passa; (3) senão, recurso ∈ conjunto de escopo.
- **Papéis→perms:** `Roles`+`RolesPermissions`+`UsersRoles` → dict via SP `GetInternalUserPermissions` (`GroupBy(PermissionCode)` = OR).
- **Imposição por operação** (não central): **(A) filtro-de-query** (List; recorta ao escopo; vazio→lista vazia) e **(B) check por-item** (Get/Write; `ValidateHotelAccess` no topo → `Unauthorized`). Fail-closed.

## C — DECISÃO: auth RICA do Decco (vertical de 1ª classe, opt-in)
Mesma cascata Omnibees, enriquecida com **duas dimensões que já existem no DeccoDB**:
- **Clearance-level** (hierárquico) ← `Cat_ClasseObjeto.NivelAcessoMinimo` (user.Clearance ≥ classe.min).
- **Sítio** (conjunto) ← `Anomalia.SitioContencao` (site ∈ user.Sites, ou bypass `AccessToAllSites`).
- **Papéis** (Pesquisador/Agente/Diretor/O5) → PermissionCodes por operação CRUD-E.
- **Onde:** core (Decco.API) = tabelas User/Role/Permission/UserRole/UserSite + Clearance + SP `sp_User_GetPermissions` **+ emissor de token** (o que a Omnibees terceiriza ao AuthServer — aqui você constrói); conector (Foundation.API) = `ApiPermissions` (EF code-first) + valida JWT + impõe no envelope.
- **Dados sensíveis (seguro):** hash com **`PasswordHasher<T>` do Identity** (PBKDF2 + salt RNG) — **não** o salt=GUID; cifra-at-rest via **`IDataProtector`** (análogo .NET do Vault Transit) atrás de `ISecretProtector`; chave JWT/segredos em **user-secrets/env vars**; **sem cartão**.
- **Imposição:** List = Padrão A (filtro por clearance+sítio); Get/Update/Delete = Padrão B (**resource-based authorization** — `AuthorizationHandler<ClearanceRequirement, Anomalia>`). Fail-closed.
- **Comparação com Identity (o Paulo domina):** Decco **não** tem AuthServer externo → valida senha e emite token **localmente** (é onde o Identity brilharia); pode-se usar o `PasswordHasher`/partes do Identity **sem** adotar o Identity inteiro. Identity é abordagem *diferente* da Omnibees (user-store local) — aqui combinamos: hash à la Identity + modelo de permissões/clearance à la Omnibees.
- **Tier:** vertical de auth = **opt-in, depois do slice Tier 0 da Anomalia** (para haver o que proteger); não precisa esperar o resto do Tier 2. Design `reference/13`; build `recipes/02`.

## D — DECISÃO: autossuficiência da skill
A skill deve **gerar os projetos/código sozinha em qualquer modelo/ambiente**, sem depender desta conversa. Regra:
- Os **padrões essenciais vivem nas `reference/`/`recipes/`** (com shapes/código concretos — ex.: `recipes/02` traz entidades, JWT, hasher, handler de recurso, seed).
- `C:\Git`, `Downloads/Decco-blueprint-0*.md` e `omni-src` são **proveniência opcional**, **não dependências** — se não existirem no ambiente, a skill ainda funciona a partir do que está nela.
- Ao gerar, **assumir por convenção** o que estiver nas reference/recipes e **declarar assunções**; marcar decisões abertas com `// >>>`.

## Como aplicar
- Auth NÃO no Tier 0. Quando o Paulo pedir a auth, seguir `recipes/02` (A→E, um passo por vez, com checkpoints). Usar `PasswordHasher`/PBKDF2 (nunca salt=GUID), JWT de dev com chave em user-secrets, resource-based authorization para "nem todos veem todas as anomalias". **Sem cartão.**
- Tratar as reference/recipes como fonte autossuficiente; não exigir os repos/zips locais.

## Fio a puxar
Emissor de token: JWT de dev (rápido) vs **Duende IdentityServer/Keycloak** local (fiel-pesado) — o que muda no cliente e na validação? (candidato a `open-questions/`).
