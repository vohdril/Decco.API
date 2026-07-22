using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Operations;

public class AnomaliaManager : IAnomaliaManager
{
    private readonly IAnomaliaRepository _repo;

    public AnomaliaManager(IAnomaliaRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<AnomaliaDto>> GetAnomalia(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null)
            return new SingleResponse<AnomaliaDto>
            {
                Status = ResponseStatus.Fail,
                Error = new ErrorInfo { Code = "NOT_FOUND", Message = "Anomalia não encontrada" }
            };

        return new SingleResponse<AnomaliaDto> { Data = MapToDto(entity) };
    }

    public async Task<PagedResponse<AnomaliaDto>> ListAnomalias(int page = 0, int pageSize = 50)
    {
        var entities = await _repo.ListAsync();
        var total = entities.Count;
        var items = entities.Skip(page * pageSize).Take(pageSize).Select(MapToDto).ToList();

        return new PagedResponse<AnomaliaDto>
        {
            Data = items,
            PageIndex = page,
            PageSize = pageSize,
            TotalRecords = total
        };
    }

    public async Task<SingleResponse<int>> InsertAnomalia(AnomaliaDto dto)
    {
        var entity = MapToEntity(dto);
        var id = await _repo.InsertAsync(entity);
        return new SingleResponse<int> { Data = id };
    }

    public async Task<SingleResponse<bool>> UpdateAnomalia(AnomaliaDto dto)
    {
        var existing = await _repo.GetByIdAsync(dto.Id);
        if (existing == null)
            return new SingleResponse<bool>
            {
                Status = ResponseStatus.Fail,
                Error = new ErrorInfo { Code = "NOT_FOUND", Message = "Anomalia não encontrada" }
            };

        await _repo.UpdateAsync(MapToEntity(dto));
        return new SingleResponse<bool> { Data = true };
    }

    public async Task<SingleResponse<bool>> DeleteAnomalia(int id)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing == null)
            return new SingleResponse<bool>
            {
                Status = ResponseStatus.Fail,
                Error = new ErrorInfo { Code = "NOT_FOUND", Message = "Anomalia não encontrada" }
            };

        await _repo.DeleteAsync(id);
        return new SingleResponse<bool> { Data = true };
    }

    private static AnomaliaDto MapToDto(Anomalium entity) => new()
    {
        Id = entity.Id,
        CodigoSCP = entity.CodigoScp,
        NomeComum = entity.NomeComum,
        Descricao = entity.Descricao,
        ClasseObjeto = entity.ClasseObjeto?.Nome ?? string.Empty,
        CamadaOntologica = entity.CamadaOntologica?.Nome ?? string.Empty,
        TipoMateria = entity.TipoMateria?.Nome ?? string.Empty,
        MecanismoPrimario = entity.MecanismoPrimario?.Nome ?? string.Empty,
        MecanismoSecundario = entity.MecanismoSecundario?.Nome,
        IEIA_D_Base = entity.IeiaDBase,
        FatorCoerenciaSpin = entity.FatorCoerenciaSpin,
        Status = entity.Status ?? "ATIVA",
        SitioContencao = entity.SitioContencao,
        ResponsavelPesquisa = entity.ResponsavelPesquisa,
        DataCriacao = entity.DataCriacao ?? DateTime.Now,
        DataAtualizacao = entity.DataAtualizacao ?? DateTime.Now
    };

    private static Anomalium MapToEntity(AnomaliaDto dto) => new()
    {
        Id = dto.Id,
        CodigoScp = dto.CodigoSCP,
        NomeComum = dto.NomeComum,
        Descricao = dto.Descricao,
        IeiaDBase = dto.IEIA_D_Base,
        FatorCoerenciaSpin = dto.FatorCoerenciaSpin,
        Status = dto.Status,
        SitioContencao = dto.SitioContencao,
        ResponsavelPesquisa = dto.ResponsavelPesquisa
    };
}
