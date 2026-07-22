using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.Data.SqlClient;
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

    public async Task<List<Anomalium>> ListAsync()
    {
        return await _ctx.Anomalia
            .Include(a => a.ClasseObjeto)
            .Include(a => a.CamadaOntologica)
            .Include(a => a.TipoMateria)
            .Include(a => a.MecanismoPrimario)
            .Include(a => a.MecanismoSecundario)
            .ToListAsync();
    }

    public async Task<Anomalium?> GetByIdAsync(int id)
    {
        return await _ctx.Anomalia
            .Include(a => a.ClasseObjeto)
            .Include(a => a.CamadaOntologica)
            .Include(a => a.TipoMateria)
            .Include(a => a.MecanismoPrimario)
            .Include(a => a.MecanismoSecundario)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> InsertAsync(Anomalium anomalia)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@CodigoSCP", anomalia.CodigoScp);
        p.Add("@NomeComum", anomalia.NomeComum);
        p.Add("@Descricao", anomalia.Descricao);
        p.Add("@ClasseObjetoId", anomalia.ClasseObjetoId);
        p.Add("@CamadaOntologicaId", anomalia.CamadaOntologicaId);
        p.Add("@TipoMateriaId", anomalia.TipoMateriaId);
        p.Add("@MecanismoPrimarioId", anomalia.MecanismoPrimarioId);
        p.Add("@MecanismoSecundarioId", anomalia.MecanismoSecundarioId);
        p.Add("@IEIA_D_Base", anomalia.IeiaDBase);
        p.Add("@FatorCoerenciaSpin", anomalia.FatorCoerenciaSpin);
        p.Add("@SitioContencao", anomalia.SitioContencao);
        p.Add("@ResponsavelPesquisa", anomalia.ResponsavelPesquisa);

        var result = await conn.QueryAsync<int>(
            "sp_Anomalia_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Anomalium anomalia)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", anomalia.Id);
        p.Add("@NomeComum", anomalia.NomeComum);
        p.Add("@Descricao", anomalia.Descricao);
        p.Add("@ClasseObjetoId", anomalia.ClasseObjetoId);
        p.Add("@CamadaOntologicaId", anomalia.CamadaOntologicaId);
        p.Add("@TipoMateriaId", anomalia.TipoMateriaId);
        p.Add("@MecanismoPrimarioId", anomalia.MecanismoPrimarioId);
        p.Add("@MecanismoSecundarioId", anomalia.MecanismoSecundarioId);
        p.Add("@IEIA_D_Base", anomalia.IeiaDBase);
        p.Add("@FatorCoerenciaSpin", anomalia.FatorCoerenciaSpin);
        p.Add("@Status", anomalia.Status);
        p.Add("@SitioContencao", anomalia.SitioContencao);
        p.Add("@ResponsavelPesquisa", anomalia.ResponsavelPesquisa);

        await conn.ExecuteAsync("sp_Anomalia_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Anomalia.FindAsync(id);
        if (entity != null)
        {
            _ctx.Anomalia.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
