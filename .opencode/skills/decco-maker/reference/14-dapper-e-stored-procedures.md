# 14 — Dapper + Stored Procedures (didático) e database-first sem perder OOP

> Referência didática para quem **pouco domina Dapper**. Ensina o conceito + como aplicar às SPs reais do `decco.sql`
> (embutido em `assets/decco.sql`). É o estudo de "execução de rotinas/stored procedures" que o Paulo quer nos slices.
> Regra da skill: EF primeiro (conforto), **Dapper depois** (rampa didática — ver `recipes/01`).

## 1. EF Core vs Dapper — quando cada um
- **EF Core** = ORM completo: mapeia tabelas↔classes, faz *change tracking*, LINQ, navegações, `SaveChanges`. Bom para **CRUD e leituras por objeto**. É onde você usa OOP à vontade.
- **Dapper** = *micro-ORM*: você escreve o **SQL** (ou o **nome da SP**) e o Dapper só **materializa o resultado num POCO** (mapeia coluna→propriedade por nome). Sem tracking, muito rápido. Bom para **stored procedures, relatórios e consultas que já vivem no banco**.
- **Híbrido (o padrão do OB.API e do Decco):** os dois **sobre a MESMA conexão** — `ctx.Database.GetDbConnection()`. EF para CRUD; Dapper para as `sp_*`.

## 2. Os 3 verbos do Dapper (é só isto)
```csharp
using Dapper;                         // pacote: Dapper
using var conn = ctx.Database.GetDbConnection();   // reusa a conexão do EF (precisa MultipleActiveResultSets=True)
var p = new DynamicParameters();
p.Add("@ClasseObjetoId", classeId, DbType.Int32);   // parâmetros nomeados (contra SQL injection: sempre parametrizar)

// (a) Query<T>  -> lista/único, mapeia colunas -> propriedades de T
var linhas = await conn.QueryAsync<AnomaliaBuscaQR>("sp_Anomalia_Buscar", p, commandType: CommandType.StoredProcedure);

// (b) Execute   -> sem retorno de linhas (INSERT/UPDATE/DELETE ou SP que não devolve SELECT)
await conn.ExecuteAsync("sp_Pericia_AdicionarManifestacao", p, commandType: CommandType.StoredProcedure);

// (c) QueryMultiple -> SP com VÁRIOS result sets; ler na ORDEM
using var multi = await conn.QueryMultipleAsync("sp_Anomalia_ObterPerfilCompleto", p, commandType: CommandType.StoredProcedure);
var info      = await multi.ReadFirstOrDefaultAsync<AnomaliaPerfilQR>();
var entidades = (await multi.ReadAsync<EntidadeVivaQR>()).ToList();
// ...ler os demais SELECTs na mesma ordem em que a SP os emite
```
- **`commandType: CommandType.StoredProcedure`** é o que diz "isto é o NOME de uma SP", não SQL cru.
- **`DynamicParameters`** = os `@parametros` da SP; para defaults, simplesmente não adicione (a SP usa o default).
- **POCO `*QR`** (sufixo "Query Result") = classe só-propriedades que **espelha as colunas do SELECT da SP** (Dapper casa por nome). Vive fora das entidades EF.

## 3. Mapa das SPs do DeccoDB → como chamar (assets/decco.sql)
| SP | Result sets | Verbo Dapper | POCO / retorno |
|---|---|---|---|
| `sp_Anomalia_Buscar` | **2**: página + `SELECT COUNT(*) as TotalRegistros` | `QueryMultiple` (página → `AnomaliaBuscaQR`; total → `int`) | lista + `TotalRecords` (→ envelope paginado) |
| `sp_Anomalia_ObterPerfilCompleto` | **7** (anomalia, entidades, artefatos, localidades, eventos, perícias, incidentes) | `QueryMultiple` + `Read<T>` em ordem | monta o perfil completo |
| `sp_Anomalia_Inserir` | 1 (`NovoId`, `CodigoFormatado`) | `QuerySingle`/`Query` | `Result` do envelope (o id novo) |
| `sp_Anomalia_Atualizar` | 0 (`@@ROWCOUNT`) | `Execute` | ok/erro |
| `sp_EntidadeViva_Inserir` / `sp_Artefato_Inserir` | 1 (`SCOPE_IDENTITY`) | `QuerySingle<long>` | id novo |
| `sp_Anomalia_AdicionarPericia` | 1 (`NovoId`) | `QuerySingle` | id da perícia |
| `sp_Pericia_AdicionarManifestacao` | 0 | `Execute` | ok/erro (N:N) |
| `sp_Incidente_Registrar` | 1 (`IncidenteId`) | `QuerySingle` | id do incidente |

> As SPs `sp_Anomalia_Inserir` etc. **encapsulam a transação/validação no banco** (há `TRY/CATCH`+`THROW` e os `TR_*` triggers). Do lado C#, o erro do SQL sobe como `SqlException` → mapear para o envelope (`Status.Fail` + `Error`), como o `SqlToDataLayerException` do OB.Api.Core (`reference/08`).

## 4. Armadilhas (as que pegam quem está a aprender)
- **Nome de coluna ≠ nome da propriedade** → Dapper não mapeia (fica default). Alinhe os nomes ou use alias no SELECT da SP.
- **Conexão compartilhada com EF** exige `MultipleActiveResultSets=True` na connection string (para EF e Dapper coexistirem).
- **Transação:** se dentro de uma transação EF, passe a `IDbTransaction` ao Dapper (`conn.Execute(sql, p, transaction: tx, ...)`).
- **Async sempre** (`QueryAsync`/`ExecuteAsync`); nunca `.Result`/`.Wait()` (ThreadPool starvation — ver `Downloads/estudo-02` se existir).
- **Parametrizar sempre** (`@param`) — nunca concatenar valores no SQL (injection).

## 5. database-first NÃO custa a OOP (importante para o seu receio)
Fazer o **domínio database-first** (rodar o `decco.sql`, banco já existe) **não** te tira orientação a objetos: o
`dotnet ef dbcontext scaffold` **gera as classes C#** (`Anomalia`, `Cat_*`…) **a partir do banco** — você continua a codar em
OOP contra essas entidades. O que você **abre mão** é de *escrever o schema em C#* (migrations); o que você **ganha** é poder
estudar **SPs/triggers/views** — que o code-first **não** expressa. Por isso o Decco é **híbrido** (ver `knowledge-drops/009`):
**domínio database-first** (SP/Dapper aqui) + **auth code-first** (seu conforto OOP, tabelas novas).

## Porquê / fio a puxar
**Porquê Dapper e não só EF para as SPs:** o EF Core sabe chamar SP (`FromSqlRaw`/`SqlQuery`), mas fica desajeitado com
**múltiplos result sets** e mapeamento fora da entidade — exatamente o caso do `sp_Anomalia_ObterPerfilCompleto` (7 SELECTs).
Dapper resolve isso com `QueryMultiple` de forma limpa. 🔎 **Fio:** implemente a List primeiro com **EF/LINQ** e depois troque por
`sp_Anomalia_Buscar` via **Dapper** — compare o SQL gerado (EF) vs a SP (controlo total) e meça a diferença.
