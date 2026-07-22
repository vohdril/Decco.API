using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

public class LaboratorioRepository : ILaboratorioRepository
{
    private readonly DeccoDbContext _ctx;

    public LaboratorioRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Laboratorio>> ListAsync()
    {
        return await _ctx.Laboratorios.ToListAsync();
    }

    public async Task<Laboratorio?> GetByIdAsync(int id)
    {
        return await _ctx.Laboratorios.FindAsync(id);
    }

    public async Task<int> InsertAsync(Laboratorio laboratorio)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Codigo", laboratorio.Codigo);
        p.Add("@Nome", laboratorio.Nome);
        p.Add("@Descricao", laboratorio.Descricao);
        p.Add("@Sitio", laboratorio.Sitio);
        p.Add("@Responsavel", laboratorio.Responsavel);
        p.Add("@Especialidade", laboratorio.Especialidade);
        p.Add("@NivelAcessoMinimo", laboratorio.NivelAcessoMinimo);

        var result = await conn.QueryAsync<int>(
            "sp_Laboratorio_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Laboratorio laboratorio)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", laboratorio.Id);
        p.Add("@Codigo", laboratorio.Codigo);
        p.Add("@Nome", laboratorio.Nome);
        p.Add("@Descricao", laboratorio.Descricao);
        p.Add("@Sitio", laboratorio.Sitio);
        p.Add("@Responsavel", laboratorio.Responsavel);
        p.Add("@Especialidade", laboratorio.Especialidade);
        p.Add("@NivelAcessoMinimo", laboratorio.NivelAcessoMinimo);
        p.Add("@Status", laboratorio.Status);

        await conn.ExecuteAsync("sp_Laboratorio_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Laboratorios.FindAsync(id);
        if (entity != null)
        {
            _ctx.Laboratorios.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
