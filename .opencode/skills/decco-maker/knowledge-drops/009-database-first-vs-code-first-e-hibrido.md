# 009 — Orientação database-first (verificada) + decisão híbrida + autossuficiência do banco

- **Data de registo:** 2026-07-13
- **Fonte:** verificação read-only (Partners `PartnersApiDbContext`/`Domains/Generated`/sem Migrations; OB.API EDMX) + pergunta do Paulo (database-first vs code-first, Identity, Dapper/SP, autossuficiência)
- **Tipo:** conhecimento resolvido + decisão de rumo
- **Afeta:** `reference/02`, `reference/14` (novo), `recipes/01`, `recipes/02`, `open-questions/Q-002`, autossuficiência

## A — Verdict: o ecossistema Omnibees é **database-first orientado** (nenhum usa migrations)
- **OB.API (legado):** **database-first puro** — EDMX + `OnModelCreating → UnintentionalCodeFirstException`; tabelas (`Users`, `Roles`, `ApiPermissions`…) feitas à mão no banco.
- **Partners (moderno):** entidades em `Partners.Api.Model/Domains/Generated/` com marcadores de **scaffold** (`#nullable disable`, `ADbDomainBase`) = **reverse-engineered de um banco existente (database-first via scaffold)**; **NÃO há pasta `Migrations`**; `EnsureCreated()` + `HasData` são só **bootstrap de DEV** do banco local minúsculo (2 tabelas). Não é code-first-com-migrations.
- **Corolário (resolve o receio do Identity):** o ASP.NET **Identity** tem fluxo canônico **code-first + migrations** (tabelas `AspNet*` via `Add-Migration`/`Update-Database`) — exatamente o mecanismo que a Omnibees **evita**. É por isso que a Omnibees **não usa Identity**: ele arrasta o mundo code-first/migração, que colide com o database-first + SPs/triggers.

## B — DECISÃO: Decco é **HÍBRIDO** (fiel à Omnibees e ótimo para os teus estudos)
- **Domínio (Decco.API core) = database-first.** O banco **já existe** — roda-se o `assets/decco.sql` (embutido na skill) **uma vez**. Entidades via `dotnet ef dbcontext scaffold` (**gera classes C# → OOP preservada**) ou mapeadas à mão. **SPs via Dapper** (`reference/14`). É AQUI que vive o estudo de banco (rotinas/SPs/views/triggers).
- **Auth = code-first.** As tabelas não existem no DeccoDB e são o teu conforto (OOP + adjacente ao Identity que dominas) → define-se as entidades e o EF cria (`EnsureCreated`/script). Espelha o Partners (`ApiPermission` é EF code-first).
- **Isto é exatamente o que a Omnibees faz:** domínio database-first (EDMX/scaffold) + a `ApiPermission` do conector code-first.

## C — Reframe (o ponto que dissolve o "database-first quebra minha OOP/autossuficiência")
- **database-first ≠ perder OOP:** o scaffold gera as classes C#; codas em OOP contra elas. Só não escreves o *schema* em C#.
- **A skill NÃO gera o schema do domínio a partir de código.** Ela **fornece/roda o `decco.sql`** (database-first). "Gerar banco" só se aplica às **tabelas de auth** (code-first, opcional).
- **Autossuficiência resolvida:** a skill agora **carrega `assets/decco.sql`** → em qualquer modelo/ambiente dá para criar+semear o DeccoDB sem depender de ficheiros externos. (Antes o `.sql` era externo — furo corrigido.)

## D — Dapper/SP é objetivo de aprendizagem explícito
O Paulo pouco domina Dapper → o estudo de SP/rotinas entra **em etapa própria, depois do conforto com EF** (rampa didática). Guia em `reference/14`; sequência em `recipes/01`.

## Como aplicar
- Ao gerar o core: **database-first** — Etapa 0 = rodar `assets/decco.sql`; entidades por scaffold; SPs por Dapper. NÃO propor migrations para o domínio.
- Ao gerar auth: **code-first** (entidades + `EnsureCreated`/script) — `recipes/02`.
- Se o Paulo perguntar "gera o banco?": **domínio = não** (database-first, roda o `decco.sql` da skill); **auth = opcional code-first**.

## Fio a puxar
Comparar, na mesma List de Anomalia, **EF/LINQ** vs **`sp_Anomalia_Buscar` via Dapper** — SQL gerado vs SP, legibilidade, performance. (Fecha o loop database-first + Dapper.)
