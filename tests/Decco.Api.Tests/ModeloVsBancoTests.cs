using Decco.Api.DataLayer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Decco.Api.Tests;

/// <summary>
/// TESTE DE ARQUITETURA — o modelo que o EF constrói corresponde ao banco que existe?
///
/// Porquê existe: num projeto database-first, a divergência entre o modelo e o schema
/// falha em SILÊNCIO até alguém executar a consulta. Foi assim que o typo "Anomalium"
/// (o scaffold pluralizou "Anomalia" em latim) sobreviveu à refatoração do
/// knowledge-drops/020: ao trocar as propriedades DbSet&lt;T&gt; por Set&lt;T&gt;(), o EF
/// perdeu o nome da tabela — que vinha do nome da PROPRIEDADE — e passou a usar o nome
/// do tipo CLR. As consultas viraram "SELECT ... FROM [Anomalium]" e os services,
/// com catch sem log, devolviam um INTERNAL_ERROR mudo.
///
/// Nenhum teste de unidade pega isso: o mock do repositório nunca toca o banco.
/// Este teste é a costura exata onde database-first quebra.
/// </summary>
public class ModeloVsBancoTests
{
    private readonly ITestOutputHelper _saida;

    public ModeloVsBancoTests(ITestOutputHelper saida) => _saida = saida;

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("DECCO_DB_CONNECTION")
        ?? @"Server=(localdb)\MSSQLLocalDB;Database=DeccoDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

    private static DeccoDbContext CriarContexto() =>
        new(new DbContextOptionsBuilder<DeccoDbContext>().UseSqlServer(ConnectionString).Options);

    [Fact]
    public void Toda_entidade_do_modelo_aponta_para_uma_tabela_ou_view_que_existe()
    {
        using var ctx = CriarContexto();
        var conn = (SqlConnection)ctx.Database.GetDbConnection();
        conn.Open();

        var ausentes = new List<string>();

        foreach (var entidade in ctx.Model.GetEntityTypes().OrderBy(e => e.ClrType.Name))
        {
            var objeto = entidade.GetTableName() ?? entidade.GetViewName();

            if (objeto is null)
            {
                ausentes.Add($"{entidade.ClrType.Name} → (sem tabela nem view mapeada)");
                continue;
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN OBJECT_ID(@nome) IS NULL THEN 0 ELSE 1 END";
            cmd.Parameters.AddWithValue("@nome", objeto);

            var existe = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            _saida.WriteLine($"{(existe ? "ok  " : "FALHA")}  {entidade.ClrType.Name,-32} → {objeto}");

            if (!existe)
                ausentes.Add($"{entidade.ClrType.Name} → '{objeto}' não existe no banco");
        }

        Assert.True(
            ausentes.Count == 0,
            $"{ausentes.Count} entidade(s) mapeiam para objetos inexistentes:{Environment.NewLine}  "
            + string.Join(Environment.NewLine + "  ", ausentes));
    }

    [Fact]
    public void Toda_propriedade_mapeada_corresponde_a_uma_coluna_que_existe()
    {
        using var ctx = CriarContexto();
        var conn = (SqlConnection)ctx.Database.GetDbConnection();
        conn.Open();

        var ausentes = new List<string>();

        foreach (var entidade in ctx.Model.GetEntityTypes())
        {
            var objeto = entidade.GetTableName() ?? entidade.GetViewName();
            if (objeto is null) continue;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT c.name
                  FROM sys.columns c
                 WHERE c.object_id = OBJECT_ID(@nome)
                """;
            cmd.Parameters.AddWithValue("@nome", objeto);

            var colunasNoBanco = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var r = cmd.ExecuteReader())
                while (r.Read()) colunasNoBanco.Add(r.GetString(0));

            if (colunasNoBanco.Count == 0) continue;   // objeto ausente já é coberto pelo outro teste

            foreach (var prop in entidade.GetProperties())
            {
                var coluna = prop.GetColumnName();
                if (coluna is not null && !colunasNoBanco.Contains(coluna))
                    ausentes.Add($"{entidade.ClrType.Name}.{prop.Name} → coluna '{objeto}.{coluna}' não existe");
            }
        }

        foreach (var a in ausentes) _saida.WriteLine($"FALHA  {a}");

        Assert.True(
            ausentes.Count == 0,
            $"{ausentes.Count} propriedade(s) mapeiam para colunas inexistentes:{Environment.NewLine}  "
            + string.Join(Environment.NewLine + "  ", ausentes));
    }
}
