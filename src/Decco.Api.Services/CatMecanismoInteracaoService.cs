using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatMecanismoInteracaoService : ICatMecanismoInteracaoService
{
    private readonly ICatMecanismoInteracaoRepository _repo;
    private readonly ILogger<CatMecanismoInteracaoService> _logger;

    public CatMecanismoInteracaoService(ICatMecanismoInteracaoRepository repo, ILogger<CatMecanismoInteracaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatMecanismoInteracaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatMecanismoInteracaoDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatMecanismoInteracaoDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatMecanismoInteracaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatMecanismoInteracaoDto>();

            return new SingleResponse<CatMecanismoInteracaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatMecanismoInteracaoDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatMecanismoInteracaoDto dto)
    {
        try
        {
            return new SingleResponse<int> { Data = await _repo.InsertAsync(MapToEntity(dto)) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<int>(_logger, ex);
        }
    }

    public async Task<SingleResponse<bool>> Update(CatMecanismoInteracaoDto dto)
    {
        try
        {
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<bool>(_logger, ex);
        }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<bool>(_logger, ex);
        }
    }

    private static CatMecanismoInteracaoDto MapToDto(CatMecanismoInteracao e) => new()
    {
        Id = e.Id,
        Codigo = e.Codigo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        CamadaOntologicaId = e.CamadaOntologicaId,
        EhSubnatureza = e.EhSubnatureza
    };

    private static CatMecanismoInteracao MapToEntity(CatMecanismoInteracaoDto dto) => new()
    {
        Id = dto.Id,
        Codigo = dto.Codigo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        CamadaOntologicaId = dto.CamadaOntologicaId,
        EhSubnatureza = dto.EhSubnatureza
    };

}
