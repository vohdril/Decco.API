using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

/// <summary>
/// The list goes through a stored procedure (sp_Operacao_Buscar): it is the SCOPED
/// query — facility + children + clearance — and returns two result sets (page and
/// total), read with QueryMultiple. Get stays on EF.
/// </summary>
public class OperacaoRepository : IOperacaoRepository
{
    private readonly DeccoDbContext _ctx;

    public OperacaoRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<(List<OperacaoSummary> Items, int Total)> SearchAsync(OperacaoFilter filter)
    {
        // DbContext connection — no `using` (see AnomaliaRepository.InsertAsync).
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@InstalacaoId", filter.InstalacaoId);
        p.Add("@IncluirSubinstalacoes", filter.IncluirSubinstalacoes);
        p.Add("@TipoOperacaoId", filter.TipoOperacaoId);
        p.Add("@Status", filter.Status);
        p.Add("@AnomaliaId", filter.AnomaliaId);
        p.Add("@NivelAcessoUsuario", filter.NivelAcessoUsuario);
        p.Add("@Pagina", filter.PageIndex + 1);   // 0-based contract → 1-based stored procedure
        p.Add("@ItensPorPagina", filter.PageSize);

        using var multi = await conn.QueryMultipleAsync(
            "sp_Operacao_Buscar",
            p,
            commandType: CommandType.StoredProcedure);

        var items = (await multi.ReadAsync<OperacaoSummary>()).ToList();
        var total = await multi.ReadSingleAsync<int>();

        return (items, total);
    }

    public async Task<Operacao?> GetByIdAsync(int id)
    {
        return await _ctx.Set<Operacao>()
            .Include(o => o.TipoOperacao)
            .Include(o => o.Instalacao)
            .Include(o => o.Anomalia)
            .Include(o => o.Protocolo)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<int> InsertAsync(Operacao operacao)
    {
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Codinome", operacao.Codinome);
        p.Add("@TipoOperacaoId", operacao.TipoOperacaoId);
        p.Add("@InstalacaoId", operacao.InstalacaoId);
        p.Add("@Objetivo", operacao.Objetivo);
        p.Add("@Codigo", string.IsNullOrWhiteSpace(operacao.Codigo) ? null : operacao.Codigo);
        p.Add("@AnomaliaId", operacao.AnomaliaId);
        p.Add("@NotificacaoId", operacao.NotificacaoId);
        p.Add("@ProtocoloId", operacao.ProtocoloId);
        p.Add("@Descricao", operacao.Descricao);
        p.Add("@Prioridade", operacao.Prioridade == 0 ? 3 : operacao.Prioridade);
        p.Add("@NivelAcessoMinimo", operacao.NivelAcessoMinimo == 0 ? 1 : operacao.NivelAcessoMinimo);
        p.Add("@Responsavel", operacao.Responsavel);
        p.Add("@DataPrevisaoTermino", operacao.DataPrevisaoTermino);

        // The stored procedure returns (NovoId, CodigoFormatado); Dapper maps the first column.
        var result = await conn.QueryAsync<int>(
            "sp_Operacao_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Operacao operacao)
    {
        // Codigo and InstalacaoId are not sent: they cannot be updated (see sp_Operacao_Atualizar).
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", operacao.Id);
        p.Add("@Codinome", operacao.Codinome);
        p.Add("@TipoOperacaoId", operacao.TipoOperacaoId == 0 ? null : operacao.TipoOperacaoId);
        p.Add("@AnomaliaId", operacao.AnomaliaId);
        p.Add("@NotificacaoId", operacao.NotificacaoId);
        p.Add("@ProtocoloId", operacao.ProtocoloId);
        p.Add("@Objetivo", operacao.Objetivo);
        p.Add("@Descricao", operacao.Descricao);
        p.Add("@Status", operacao.Status);
        p.Add("@Prioridade", operacao.Prioridade == 0 ? null : operacao.Prioridade);
        p.Add("@NivelAcessoMinimo", operacao.NivelAcessoMinimo == 0 ? null : operacao.NivelAcessoMinimo);
        p.Add("@Responsavel", operacao.Responsavel);
        p.Add("@DataPrevisaoTermino", operacao.DataPrevisaoTermino);
        p.Add("@DataEncerramento", operacao.DataEncerramento);
        p.Add("@ResultadoResumo", operacao.ResultadoResumo);

        await conn.ExecuteAsync("sp_Operacao_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Set<Operacao>().FindAsync(id);
        if (entity != null)
        {
            _ctx.Set<Operacao>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
