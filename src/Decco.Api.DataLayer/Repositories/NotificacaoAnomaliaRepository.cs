using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

public class NotificacaoAnomaliaRepository : INotificacaoAnomaliaRepository
{
    private readonly DeccoDbContext _ctx;

    public NotificacaoAnomaliaRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<NotificacaoAnomalia>> ListAsync()
    {
        return await _ctx.Set<NotificacaoAnomalia>().ToListAsync();
    }

    public async Task<NotificacaoAnomalia?> GetByIdAsync(int id)
    {
        return await _ctx.Set<NotificacaoAnomalia>().FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<int> InsertAsync(NotificacaoAnomalia notificacao)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Titulo", notificacao.Titulo);
        p.Add("@Descricao", notificacao.Descricao);
        p.Add("@LocalIdentificado", notificacao.LocalIdentificado);
        p.Add("@NivelPrioridade", notificacao.NivelPrioridade);
        p.Add("@Relator", notificacao.Relator);
        p.Add("@AnomaliaId", notificacao.AnomaliaId);

        var result = await conn.QueryAsync<int>(
            "sp_NotificacaoAnomalia_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(NotificacaoAnomalia notificacao)
    {
        using var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", notificacao.Id);
        p.Add("@Titulo", notificacao.Titulo);
        p.Add("@Descricao", notificacao.Descricao);
        p.Add("@LocalIdentificado", notificacao.LocalIdentificado);
        p.Add("@NivelPrioridade", notificacao.NivelPrioridade);
        p.Add("@Status", notificacao.Status);
        p.Add("@Relator", notificacao.Relator);
        p.Add("@AnomaliaId", notificacao.AnomaliaId);

        await conn.ExecuteAsync("sp_NotificacaoAnomalia_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Set<NotificacaoAnomalia>().FindAsync(id);
        if (entity != null)
        {
            _ctx.Set<NotificacaoAnomalia>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
