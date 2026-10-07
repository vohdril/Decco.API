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

    private IQueryable<Anomalia> ComClassificacao() => _ctx.Set<Anomalia>()
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
        var itens = await ComClassificacao()
            .OrderBy(a => a.CodigoScp)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (itens, total);
    }

    public async Task<Anomalia?> GetByIdAsync(int id)
    {
        return await ComClassificacao().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<int> InsertAsync(Anomalia anomalia)
    {
        // A conexão é do DbContext: NÃO usar `using` aqui. Um `using var conn`
        // descartava a conexão do contexto e quebrava o próximo uso dele no
        // mesmo request. O Dapper abre e fecha a conexão se ela estiver fechada.
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
        // Na SP, NULL = "não alterar". Um Id obrigatório que chega 0 (não
        // informado no DTO) vira NULL, em vez de tentar gravar uma FK inexistente.
        p.Add("@ClasseObjetoId", NuloSeZero(anomalia.ClasseObjetoId));
        p.Add("@CamadaOntologicaId", NuloSeZero(anomalia.CamadaOntologicaId));
        p.Add("@TipoMateriaId", NuloSeZero(anomalia.TipoMateriaId));
        p.Add("@MecanismoPrimarioId", NuloSeZero(anomalia.MecanismoPrimarioId));
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

    private static int? NuloSeZero(int valor) => valor == 0 ? null : valor;
}
