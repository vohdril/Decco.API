using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

public class ProtocoloContencaoRepository : IProtocoloContencaoRepository
{
    private readonly DeccoDbContext _ctx;

    public ProtocoloContencaoRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<ProtocoloContencao>> ListAsync()
    {
        return await _ctx.Set<ProtocoloContencao>().ToListAsync();
    }

    public async Task<ProtocoloContencao?> GetByIdAsync(int id)
    {
        return await _ctx.Set<ProtocoloContencao>()
            .Include(p => p.ProtocoloAplicadoEms)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<int> InsertAsync(ProtocoloContencao protocolo)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Codigo", protocolo.Codigo);
        p.Add("@Titulo", protocolo.Titulo);
        p.Add("@Descricao", protocolo.Descricao);
        p.Add("@NivelUrgencia", protocolo.NivelUrgencia);
        p.Add("@ClassesAplicaveis", protocolo.ClassesAplicaveis);
        p.Add("@Passos", protocolo.Passos);
        p.Add("@RecursosNecessarios", protocolo.RecursosNecessarios);

        var result = await conn.QueryAsync<int>(
            "sp_ProtocoloContencao_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(ProtocoloContencao protocolo)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", protocolo.Id);
        p.Add("@Codigo", protocolo.Codigo);
        p.Add("@Titulo", protocolo.Titulo);
        p.Add("@Descricao", protocolo.Descricao);
        p.Add("@NivelUrgencia", protocolo.NivelUrgencia);
        p.Add("@ClassesAplicaveis", protocolo.ClassesAplicaveis);
        p.Add("@Passos", protocolo.Passos);
        p.Add("@RecursosNecessarios", protocolo.RecursosNecessarios);

        await conn.ExecuteAsync("sp_ProtocoloContencao_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Set<ProtocoloContencao>().FindAsync(id);
        if (entity != null)
        {
            _ctx.Set<ProtocoloContencao>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
