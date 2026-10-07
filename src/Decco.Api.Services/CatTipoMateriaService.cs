using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatTipoMateriaService : ICatTipoMateriaService
{
    private readonly ICatTipoMateriaRepository _repo;
    private readonly ILogger<CatTipoMateriaService> _logger;

    public CatTipoMateriaService(ICatTipoMateriaRepository repo, ILogger<CatTipoMateriaService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatTipoMateriaDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatTipoMateriaDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatTipoMateriaDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatTipoMateriaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatTipoMateriaDto>();

            return new SingleResponse<CatTipoMateriaDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatTipoMateriaDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatTipoMateriaDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatTipoMateriaDto dto)
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

    private static CatTipoMateriaDto MapToDto(CatTipoMateria e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Descricao = e.Descricao,
        IsResistenteSupressores = e.IsResistenteSupressores
    };

    private static CatTipoMateria MapToEntity(CatTipoMateriaDto dto) => new()
    {
        Id = dto.Id,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        IsResistenteSupressores = dto.IsResistenteSupressores
    };

}
