using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatPericulosidadeService : ICatPericulosidadeService
{
    private readonly ICatPericulosidadeRepository _repo;
    private readonly ILogger<CatPericulosidadeService> _logger;

    public CatPericulosidadeService(ICatPericulosidadeRepository repo, ILogger<CatPericulosidadeService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatPericulosidadeDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatPericulosidadeDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatPericulosidadeDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatPericulosidadeDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatPericulosidadeDto>();

            return new SingleResponse<CatPericulosidadeDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatPericulosidadeDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatPericulosidadeDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatPericulosidadeDto dto)
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

    private static CatPericulosidadeDto MapToDto(CatPericulosidade e) => new()
    {
        Id = e.Id,
        Nivel = e.Nivel,
        Nome = e.Nome,
        Descricao = e.Descricao,
        CorAlerta = e.CorAlerta
    };

    private static CatPericulosidade MapToEntity(CatPericulosidadeDto dto) => new()
    {
        Id = dto.Id,
        Nivel = dto.Nivel,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        CorAlerta = dto.CorAlerta
    };

}
