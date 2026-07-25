# reference/02 — Conexão com banco (Decco.API core) e configuração livre

> Fonte: dissecação de `C:\Git\bhi-ob-api` (EF6+Dapper) e `partners-api` (EF Core), + `decco.sql`.
> É aqui que você tem **liberdade total** (no mundo real a OB.API é imutável; a Decco.API é sua).

## Dois modelos reais (contraste que orienta a decisão)

| | OB.API → **modelo da Decco.API** | Conector Partners → modelo da Foundation |
|---|---|---|
| ORM | EF6 **database-first (EDMX)** | EF Core 8 (scaffold parcial) |
| SP / SQL bruto | **central** — Dapper `CommandType.StoredProcedure` | **nenhum** (domínio vem por HTTP) |
| Acesso | `Repository<T>` **híbrido** (EF `_objectSet` + Dapper `_connection`, mesma conexão) | LINQ/EF puro |
| Config | `Web.config`, pares RW/RO (`applicationintent=readonly`) | `SqlConnectionStringBuilder` + env vars + Vault |
| Schema | grande, pré-existente | `EnsureCreated()` só em Dev, sem migrations |

## Receita para a Decco.API sobre o DeccoDB (EF Core + Dapper)

O DeccoDB **tem stored procedures** (`sp_Anomalia_*`) e schema rico → combine **EF Core (CRUD/LINQ)** + **Dapper (SP)**,
o híbrido do OB.API modernizado.

| Necessidade | Tecnologia | Como |
|---|---|---|
| CRUD `Anomalia` + filhos | **EF Core** | `DeccoDbContext` + LINQ |
| `sp_Anomalia_Buscar` (paginado + `TotalRegistros`) | **Dapper** | `conn.Query<AnomaliaBuscaQR>("sp_Anomalia_Buscar", p, commandType: StoredProcedure)` |
| `sp_Anomalia_ObterPerfilCompleto` (vários SELECT) | **Dapper** | `QueryMultiple(...)` lê cada result set em ordem |
| `sp_Anomalia_Inserir` (retorna `NovoId`) | **Dapper** | `Query<int>(...).Single()` → vira `Result` do envelope |
| Views `vw_*` | EF Core keyless **ou** Dapper | read-models |
| Catálogos `Cat_*` (quentes, read-only) | EF Core + `ICacheProvider` | cache-aside |

**Repositório híbrido (a peça-chave):** injetar `DeccoDbContext`; `_set = ctx.Set<T>()` (EF) e
`_conn = ctx.Database.GetDbConnection()` (Dapper) — **mesma conexão física**.
- **Porquê a mesma conexão:** evita conexões duplicadas e permite participar da mesma transação. *Alternativa rejeitada:*
  abrir uma `SqlConnection` separada para o Dapper — duplica pool, complica transação, e foi o que o OB.API **não** fez.
- **Porquê POCOs `*QR`:** o shape de uma SP quase nunca casa 1:1 com a entidade EF; um POCO dedicado (sufixo `QR`) evita
  forçar a entidade a ter campos que só existem no resultado da SP.

## Configuração da conexão (a liberdade que você quer)
- **Simples no sandbox:** string em `appsettings.json` + `IConfiguration` direto. **Evoluir para:** `SqlConnectionStringBuilder`
  + env vars (como o Partners) para separar segredo de config.
- **Ambiente de banco:** **default do Tier 0 = LocalDB** (`(localdb)\MSSQLLocalDB`) — o que o operador domina; alternativas: instância SQL Server existente ou **Docker** (`mcr.microsoft.com/mssql/server`). A **skill provisiona o banco**: a Etapa 0 checa `DB_ID('DeccoDB')` e, se **não existir**, roda o **`assets/decco.sql`** (carregado na skill → cria tabelas + SPs + views + triggers + 2 anomalias). Se já existir, não re-roda (o script não é idempotente). Detalhe operacional em `recipes/03`.
  - **Config trocável num único ponto:** `ConnectionStrings:DeccoDb` (em `appsettings.Development.json`, sobreposta por user-secrets/env var) — trocar **LocalDB ⇄ Docker ⇄ remoto** é mudar **essa linha**. **MARS obrigatório** (`MultipleActiveResultSets=True`) para o híbrido EF+Dapper coexistir.
  - **1ª etapa de aprendizagem guiada:** começar em **LocalDB** e depois **migrar o mesmo banco para Docker** trocando só a connection string (a skill guia o `docker run` + re-executar o `assets/decco.sql`) — prova a trocabilidade.
- **Sem multi-tenant** → **um único contexto** (bem mais simples que os 12 EDMX do OB.API). `SessionFactory`/`UnitOfWork` são
  opcionais no início — adicione depois como exercício de abstração.
  > Os **fontes reais** dessas abstrações já estão dissecados: versão **legada** (EF6) em `reference/08` (OB.Api.Core:
  > `IObjectContext`/`ISessionFactory`/`UnitOfWorkBase`/`DomainScope`) e versão **moderna** (EF Core) em `reference/09`
  > (OB.Api.Base.DataLayer.EF: `UnitOfWork<TContext>`/`SessionFactory<TContext>`/`SqlRepositoryBase<T>`). Use como molde.
- **Database-first vs code-first:** decisão pedagógica. *Scaffold* do DeccoDB (database-first, como o OB.API) **ou** code-first
  com migrations (o que o Partners **não** faz). Comparar os dois é dos melhores aprendizados aqui.

## Quando usar EF vs Dapper vs SP (quadro de decisão)
- **Leitura simples / CRUD** → EF Core (LINQ, tracking, navegações).
- **Consulta/relatório pesada, ou lógica já no banco (SP)** → Dapper `StoredProcedure`.
- **Escrita transacional multi-tabela** → EF Core + `SaveChanges()` numa transação; ou a própria SP se ela já encapsula a transação.

## Fio a puxar 🔎
Rode `sp_Anomalia_ObterPerfilCompleto` no SSMS e conte os result sets. Como você mapearia isso com `QueryMultiple` do Dapper
vs montando o perfil com N repositórios EF? Qual é mais legível? Mais rápido? (Experimento → registre em `open-questions/`.)
