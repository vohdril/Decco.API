using Dapper;
using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Decco.Api.DataLayer.Repositories;

/// <summary>
/// Leitura por EF (com tipo e pai), escrita por stored procedure — mesmo padrão
/// híbrido de Anomalia. As regras de hierarquia ficam no banco
/// (TR_Instalacao_Validar_Hierarquia); aqui não se duplica regra.
/// </summary>
public class InstalacaoRepository : IInstalacaoRepository
{
    private readonly DeccoDbContext _ctx;

    public InstalacaoRepository(DeccoDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<List<Instalacao>> ListAsync()
    {
        return await _ctx.Set<Instalacao>()
            .Include(i => i.TipoInstalacao)
            .Include(i => i.InstalacaoPai)
            .OrderBy(i => i.Codigo)
            .ToListAsync();
    }

    public async Task<Instalacao?> GetByIdAsync(int id)
    {
        return await _ctx.Set<Instalacao>()
            .Include(i => i.TipoInstalacao)
            .Include(i => i.InstalacaoPai)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<int> InsertAsync(Instalacao instalacao)
    {
        // Conexão do DbContext — sem `using` (ver AnomaliaRepository.InsertAsync).
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Codigo", instalacao.Codigo);
        p.Add("@Nome", instalacao.Nome);
        p.Add("@TipoInstalacaoId", instalacao.TipoInstalacaoId);
        p.Add("@InstalacaoPaiId", instalacao.InstalacaoPaiId);
        p.Add("@Descricao", instalacao.Descricao);
        p.Add("@Responsavel", instalacao.Responsavel);
        p.Add("@Especialidade", instalacao.Especialidade);
        p.Add("@NivelAcessoMinimo", instalacao.NivelAcessoMinimo == 0 ? 1 : instalacao.NivelAcessoMinimo);

        var result = await conn.QueryAsync<int>(
            "sp_Instalacao_Inserir",
            p,
            commandType: CommandType.StoredProcedure);

        return result.Single();
    }

    public async Task UpdateAsync(Instalacao instalacao)
    {
        // Codigo não vai: não é atualizável (identidade pública e chave de cache).
        var conn = _ctx.Database.GetDbConnection();
        var p = new DynamicParameters();
        p.Add("@Id", instalacao.Id);
        p.Add("@Nome", instalacao.Nome);
        p.Add("@Descricao", instalacao.Descricao);
        p.Add("@TipoInstalacaoId", instalacao.TipoInstalacaoId == 0 ? null : instalacao.TipoInstalacaoId);
        p.Add("@InstalacaoPaiId", instalacao.InstalacaoPaiId);
        p.Add("@Responsavel", instalacao.Responsavel);
        p.Add("@Especialidade", instalacao.Especialidade);
        p.Add("@NivelAcessoMinimo", instalacao.NivelAcessoMinimo == 0 ? null : instalacao.NivelAcessoMinimo);
        p.Add("@Status", instalacao.Status);

        await conn.ExecuteAsync("sp_Instalacao_Atualizar", p, commandType: CommandType.StoredProcedure);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _ctx.Set<Instalacao>().FindAsync(id);
        if (entity != null)
        {
            _ctx.Set<Instalacao>().Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}
