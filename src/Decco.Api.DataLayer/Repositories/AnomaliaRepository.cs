using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

public class AnomaliaRepository : IAnomaliaRepository
{
    private readonly DeccoDbContext _ctx;

    public AnomaliaRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    private IQueryable<Anomalia> WithClassification() => _ctx.Set<Anomalia>()
        .Include(a => a.ClasseObjeto)
        .Include(a => a.CamadaOntologica)
        .Include(a => a.TipoMateria)
        .Include(a => a.CognicaoAparente)
        .Include(a => a.Periculosidade)
        .Include(a => a.MecanismoPrimario)
        .Include(a => a.MecanismoSecundario)
        .Include(a => a.InstalacaoContencao);

    public async Task<(List<Anomalia> Itens, int Total)> ListAsync(int pageIndex, int pageSize)
    {
        var total = await _ctx.Set<Anomalia>().CountAsync();
        var items = await WithClassification()
            .OrderBy(a => a.CodigoScp)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Anomalia?> GetByIdAsync(int id)
    {
        return await WithClassification().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> InsertAsync(Anomalia anomalia)
    {
        // The connection belongs to the DbContext: do NOT use `using` here. A `using var conn`
        // disposed the context connection and broke its next use within the same request.
        // Dapper opens and closes the connection when it is closed.
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@CodigoSCP", anomalia.CodigoScp);
        p.Add("@NomeComum", anomalia.NomeComum);
        p.Add("@Descricao", anomalia.Descricao);
        p.Add("@ClasseObjetoId", anomalia.ClasseObjetoId);
        p.Add("@CamadaOntologicaId", anomalia.CamadaOntologicaId);
        p.Add("@TipoMateriaId", anomalia.TipoMateriaId);
        p.Add("@CognicaoAparenteId", anomalia.CognicaoAparenteId);
        p.Add("@PericulosidadeId", anomalia.PericulosidadeId);
        p.Add("@MecanismoPrimarioId", anomalia.MecanismoPrimarioId);
        p.Add("@MecanismoSecundarioId", anomalia.MecanismoSecundarioId);
        p.Add("@IEIA_D_Base", anomalia.IeiaDBase);
        p.Add("@FatorCoerenciaSpin", anomalia.FatorCoerenciaSpin);
        p.Add("@InstalacaoContencaoId", anomalia.InstalacaoContencaoId);
        p.Add("@ResponsavelPesquisa", anomalia.ResponsavelPesquisa);

        var result = await conn.QueryAsync<int>(
            "sp_Anomalia_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Anomalia anomalia)
    {
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", anomalia.Id);
        p.Add("@NomeComum", anomalia.NomeComum);
        p.Add("@Descricao", anomalia.Descricao);
        // In the stored procedure, NULL = "do not change". A mandatory Id arriving as 0 (not
        // provided in the DTO) becomes NULL, instead of trying to write a non-existent FK.
        p.Add("@ClasseObjetoId", NullIfZero(anomalia.ClasseObjetoId));
        p.Add("@CamadaOntologicaId", NullIfZero(anomalia.CamadaOntologicaId));
        p.Add("@TipoMateriaId", NullIfZero(anomalia.TipoMateriaId));
        p.Add("@MecanismoPrimarioId", NullIfZero(anomalia.MecanismoPrimarioId));
        p.Add("@MecanismoSecundarioId", anomalia.MecanismoSecundarioId);
        p.Add("@IEIA_D_Base", anomalia.IeiaDBase);
        p.Add("@FatorCoerenciaSpin", anomalia.FatorCoerenciaSpin);
        p.Add("@Status", anomalia.Status);
        p.Add("@InstalacaoContencaoId", anomalia.InstalacaoContencaoId);
        p.Add("@CognicaoAparenteId", anomalia.CognicaoAparenteId);
        p.Add("@PericulosidadeId", anomalia.PericulosidadeId);
        p.Add("@ResponsavelPesquisa", anomalia.ResponsavelPesquisa);

        await conn.ExecuteAsync("sp_Anomalia_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Set<Anomalia>().FindAsync(id);
        if (entity != null)
        {
            _ctx.Set<Anomalia>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }

    private static int? NullIfZero(int value) => value == 0 ? null : value;
}
