using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatForcaFundamentalService : ICatForcaFundamentalService
{
    private readonly ICatForcaFundamentalRepository _repo;
    private readonly ILogger<CatForcaFundamentalService> _logger;

    public CatForcaFundamentalService(ICatForcaFundamentalRepository repo, ILogger<CatForcaFundamentalService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatForcaFundamentalDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatForcaFundamentalDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatForcaFundamentalDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatForcaFundamentalDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatForcaFundamentalDto>();

            return new SingleResponse<CatForcaFundamentalDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatForcaFundamentalDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatForcaFundamentalDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatForcaFundamentalDto dto)
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

    private static CatForcaFundamentalDto MapToDto(CatForcaFundamental e) => new()
    {
        Id = e.Id,
        Simbolo = e.Simbolo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        ParticulaPortadora = e.ParticulaPortadora,
    };

    private static CatForcaFundamental MapToEntity(CatForcaFundamentalDto dto) => new()
    {
        Id = dto.Id,
        Simbolo = dto.Simbolo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        ParticulaPortadora = dto.ParticulaPortadora,
    };

}
