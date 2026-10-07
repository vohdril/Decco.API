using Decco.Api.DataLayer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Decco.Api.Tests;

/// <summary>
/// ARCHITECTURE TEST — does the model EF builds match the database that exists?
///
/// Why it exists: in a database-first project, a mismatch between model and schema fails
/// SILENTLY until someone runs the query. That is how the "Anomalium" typo (the scaffold
/// pluralized "Anomalia" as Latin) survived the knowledge-drops/020 refactoring: when the
/// DbSet&lt;T&gt; properties were replaced by Set&lt;T&gt;(), EF lost the table name — which
/// came from the PROPERTY name — and started using the CLR type name. Queries became
/// "SELECT ... FROM [Anomalium]" and the services, with log-less catch blocks, returned a
/// mute INTERNAL_ERROR.
///
/// No unit test catches this: the repository mock never touches the database.
/// This test is the exact seam where database-first breaks.
/// </summary>
public class ModelVsDatabaseTests
{
    private readonly ITestOutputHelper _output;

    public ModelVsDatabaseTests(ITestOutputHelper output) => _output = output;

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable("DECCO_DB_CONNECTION")
        ?? @"Server=(localdb)\MSSQLLocalDB;Database=DeccoDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True;";

    private static DeccoDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<DeccoDbContext>().UseSqlServer(ConnectionString).Options);

    [Fact]
    public void Every_model_entity_points_to_an_existing_table_or_view()
    {
        using var ctx = CreateContext();
        var conn = (SqlConnection)ctx.Database.GetDbConnection();
        conn.Open();

        var missing = new List<string>();

        foreach (var entity in ctx.Model.GetEntityTypes().OrderBy(e => e.ClrType.Name))
        {
            var dbObject = entity.GetTableName() ?? entity.GetViewName();

            if (dbObject is null)
            {
                missing.Add($"{entity.ClrType.Name} → (no table or view mapped)");
                continue;
            }

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CASE WHEN OBJECT_ID(@name) IS NULL THEN 0 ELSE 1 END";
            cmd.Parameters.AddWithValue("@name", dbObject);

            var exists = Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            _output.WriteLine($"{(exists ? "ok  " : "FAIL")}  {entity.ClrType.Name,-32} → {dbObject}");

            if (!exists)
                missing.Add($"{entity.ClrType.Name} → '{dbObject}' does not exist in the database");
        }

        Assert.True(
            missing.Count == 0,
            $"{missing.Count} entity(ies) map to non-existent objects:{Environment.NewLine}  "
            + string.Join(Environment.NewLine + "  ", missing));
    }

    [Fact]
    public void Every_mapped_property_matches_an_existing_column()
    {
        using var ctx = CreateContext();
        var conn = (SqlConnection)ctx.Database.GetDbConnection();
        conn.Open();

        var missing = new List<string>();

        foreach (var entity in ctx.Model.GetEntityTypes())
        {
            var dbObject = entity.GetTableName() ?? entity.GetViewName();
            if (dbObject is null) continue;

            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT c.name
                  FROM sys.columns c
                 WHERE c.object_id = OBJECT_ID(@name)
                """;
            cmd.Parameters.AddWithValue("@name", dbObject);

            var databaseColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var r = cmd.ExecuteReader())
                while (r.Read()) databaseColumns.Add(r.GetString(0));

            if (databaseColumns.Count == 0) continue;   // a missing object is already covered by the other test

            foreach (var prop in entity.GetProperties())
            {
                var column = prop.GetColumnName();
                if (column is not null && !databaseColumns.Contains(column))
                    missing.Add($"{entity.ClrType.Name}.{prop.Name} → column '{dbObject}.{column}' does not exist");
            }
        }

        foreach (var m in missing) _output.WriteLine($"FAIL  {m}");

        Assert.True(
            missing.Count == 0,
            $"{missing.Count} property(ies) map to non-existent columns:{Environment.NewLine}  "
            + string.Join(Environment.NewLine + "  ", missing));
    }
}
