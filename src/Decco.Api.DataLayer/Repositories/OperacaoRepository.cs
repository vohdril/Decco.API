using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

/// <summary>
/// A listagem é por stored procedure (sp_Operacao_Buscar): é a consulta
/// ESCOPADA — instalação + filhas + clearance — e devolve dois result sets
/// (página e total), lidos com QueryMultiple. Get continua por EF.
/// </summary>
public class OperacaoRepository : IOperacaoRepository
{
    private readonly DeccoDbContext _ctx;

    public OperacaoRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<(List<OperacaoResumo> Itens, int Total)> BuscarAsync(OperacaoFiltro filtro)
    {
        // Conexão do DbContext — sem `using` (ver AnomaliaRepository.InsertAsync).
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@InstalacaoId", filtro.InstalacaoId);
        p.Add("@IncluirSubinstalacoes", filtro.IncluirSubinstalacoes);
        p.Add("@TipoOperacaoId", filtro.TipoOperacaoId);
        p.Add("@Status", filtro.Status);
        p.Add("@AnomaliaId", filtro.AnomaliaId);
        p.Add("@NivelAcessoUsuario", filtro.NivelAcessoUsuario);
        p.Add("@Pagina", filtro.PageIndex + 1);   // contrato base 0 → SP base 1
        p.Add("@ItensPorPagina", filtro.PageSize);

        using var multi = await conn.QueryMultipleAsync(
            "sp_Operacao_Buscar",
            p,
            commandType: CommandType.StoredProcedure);

        var itens = (await multi.ReadAsync<OperacaoResumo>()).ToList();
        var total = await multi.ReadSingleAsync<int>();

        return (itens, total);
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

        // A SP devolve (NovoId, CodigoFormatado); o Dapper mapeia a 1ª coluna.
        var result = await conn.QueryAsync<int>(
            "sp_Operacao_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Operacao operacao)
    {
        // Codigo e InstalacaoId não vão: não são atualizáveis (ver sp_Operacao_Atualizar).
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
