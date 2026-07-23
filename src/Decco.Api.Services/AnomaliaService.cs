using Decco.Api.Common;
using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class AnomaliaService : IAnomaliaService
{
    private readonly IAnomaliaRepository _repo;

    public AnomaliaService(IAnomaliaRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<AnomaliaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<AnomaliaDto>();

            return new SingleResponse<AnomaliaDto> { Data = MapToDto(entity) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<AnomaliaDto>();
        }
    }

    public async Task<PagedResponse<AnomaliaDto>> List(int page = 0, int pageSize = 50)
    {
        try
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
        catch
        {
            return new PagedResponse<AnomaliaDto>
            {
                Status = ResponseStatus.Fail,
                Error = new ErrorInfo { Code = ErrorCodes.InternalError.GetCode(), Message = ErrorCodes.InternalError.DefaultMessage }
            };
        }
    }

    public async Task<SingleResponse<int>> Insert(AnomaliaDto dto)
    {
        try
        {
            var entity = MapToEntity(dto);
            var id = await _repo.InsertAsync(entity);
            return new SingleResponse<int> { Data = id };
        }
        catch
        {
            return ErrorResponseHelper.Fail<int>();
        }
    }

    public async Task<SingleResponse<bool>> Update(AnomaliaDto dto)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                return ErrorResponseHelper.NotFound<bool>();

            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch
        {
            return ErrorResponseHelper.Fail<bool>();
        }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return ErrorResponseHelper.NotFound<bool>();

            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch
        {
            return ErrorResponseHelper.Fail<bool>();
        }
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
