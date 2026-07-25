# 025 — Validação das estruturas Partners e Omnibees vs código real

- **Data de registo:** 2026-07-25
- **Fonte:** auditoria cruzada dos 3 repositórios (Decco.Dashboard v0.0.1, Decco.API v0.0.1, decco-skill) contra as referências do `reference/` e `tierContent.ts`
- **Tipo:** correção/atualização
- **Afeta:** `tierContent.ts` (FE/BE/DB tiers), `reference/05`, `reference/06`, `reference/12`
- **Camada:** todos

## Resumo executivo

A skill descreve com **alta fidelidade** os stacks Partners (moderno) e OB.API (legado) da Omnibees nos `reference/`. As estruturas de banco (`decco.sql`), envelope, repositório híbrido EF Core+Dapper e lore brasileiro estão **corretos**. As **inconsistências estão no `tierContent.ts`** (roadmap didático do Dashboard), que mistura o que **já existe** com o que **está planejado** sem deixar isso explícito.

## Validações por camada

### 1. Partners (stack moderno) — `reference/06` ✅ MATCHES
- .NET 8, ASP.NET Core, DI nativa, FusionCache, JWT, FluentValidation, AutoMapper — **confirmado** nos fontes reais

### 2. OB.API (stack legado) — `reference/05` ✅ MATCHES
- .NET Framework 4.8, EF6/EDMX (12 contextos), Unity 4 + AOP, OWIN, Web API 2, Dapper híbrido — **confirmado**
- `OB.Api.Core` abstrações (`IObjectContext`, `ISessionFactory`, `IUnitOfWork`, `DomainScope`) — fonte dissecado em `reference/08`
- `OB.Api.Base.*` (Criteria, UoW EF Core, `SqlRepositoryBase`) — fonte dissecado em `reference/09`

### 3. Molde PAR — `reference/10` ✅ MATCHES
- PAR é o molde correto (serviço de domínio único, envelope mínimo `OB.PAR.Contracts` com 35 ficheiros)
- Decisão de replicar PAR, não o monólito, continua válida

### 4. Autenticação/permissionamento — `reference/12` ✅ MATCHES
- **NÃO usa ASP.NET Identity** em nenhum stack (confirmado por grep)
- Partners: JWT (IdentityServer externo) + `ApiPermission` EF code-first + `HasData`
- OB.API: OWIN OAuth Bearer + tabelas `Users`/`Roles`/`RolesPermissions` database-first + SP `GetInternalUserPermissions`
- **Deny-by-default** confirmado no modelo Partners

### 5. Banco de dados — `assets/decco.sql` e `reference/14` ✅ MATCHES
- Schema OA brasileiro (PACATO/YAGUARA/ABAPORU/UKAR, SE/SA/IN/AA, 9 níveis de periculosidade) — **correto e fiel**
- Stored procedures CRUD com `TRY/CATCH` e triggers de auditoria — **implementado**
- Híbrido EF Core + Dapper na mesma conexão — **confirmado** em `AnomaliaRepository.cs`

### 6. Arquitetura Decco.API — `tierContent.ts` BE0 ⚠️ PARCIAL
- .NET 8 ✅ — confirmado
- **4 projetos? ❌** — o código real tem **7 projetos**: `Decco.Api.REST`, `Decco.Contracts`, `Decco.Api.Contracts`, `Decco.Api.DataLayer`, `Decco.Api.Services`, `Decco.Api.Common`, `Decco.Api.Root`
- **Domain project? ❌** — não existe; domínios vivem em `DataLayer/Models/`
- O envelope no código real usa `ResponseStatus Status` (enum) e `ErrorInfo? Error`, mas o `tierContent.ts` BE0 mostra `bool Success` e `List<string>? Errors` — **inconsistência** com o código real

### 7. Features planejadas vs existentes — `tierContent.ts` BE1+ ⚠️ ATENÇÃO
- JWT auth + PBKDF2 — descrito como se existisse, mas **não implementado** (planejado para BE1)
- Docker Compose com SQL Server — **não existe** (usa LocalDB, planejado para BE1)
- ApiPermission + deny-by-default — **não existe** (planejado para BE1)
- Criteria pattern (`ACriteriaBase`) — **não existe** (planejado para BE2)
- FusionCache/Redis — **não existe** (planejado para BE2+)
- MongoDB/Elasticsearch/Kafka/Vault — **não existem** (planejado para DB3/BE4/DB4)

**Todos esses estão corretamente alocados nos tiers superiores**, mas o `tierContent.ts` mistura a descrição didática com exemplos de código que podem dar a impressão de que já estão implementados.

## Decisões

1. **Manter o `reference/` como está** — a fotografia factual dos stacks Partners e OB.API está correta
2. **`tierContent.ts` recebeu textos didáticos aprovados** — agora cada tópico tem `code` próprio
3. **A numeração de projetos** (4 vs 7) é uma simplificação didática aceitável, mas deve-se evitar contradizer o código real nos exemplos de código
4. **Exemplo de código do envelope corrigido** — o snippet no BE0 foi alinhado ao código real (enum `Status` + `ErrorInfo? Error`)
