using Decco.Api.Common;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatTipoMateriumService : ICatTipoMateriumService
{
    private readonly ICatTipoMateriumRepository _repo;

    public CatTipoMateriumService(ICatTipoMateriumRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatTipoMateriumDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatTipoMateriumDto>> { Data = dtos };
        }
        catch
        {
            return ErrorResponseHelper.Fail<List<CatTipoMateriumDto>>();
        }
    }

    public async Task<SingleResponse<CatTipoMateriumDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatTipoMateriumDto>();

            return new SingleResponse<CatTipoMateriumDto> { Data = MapToDto(entity) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<CatTipoMateriumDto>();
        }
    }

    public async Task<SingleResponse<int>> Insert(CatTipoMateriumDto dto)
    {
        try
        {
            return new SingleResponse<int> { Data = await _repo.InsertAsync(MapToEntity(dto)) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<int>();
        }
    }

    public async Task<SingleResponse<bool>> Update(CatTipoMateriumDto dto)
    {
        try
        {
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
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch
        {
            return ErrorResponseHelper.Fail<bool>();
        }
    }

    private static CatTipoMateriumDto MapToDto(CatTipoMaterium e) => new()
    {
        Id = e.Id,
        Nome = e.Nome,
        Descricao = e.Descricao,
        IsResistenteSupressores = e.IsResistenteSupressores
    };

    private static CatTipoMaterium MapToEntity(CatTipoMateriumDto dto) => new()
    {
        Id = dto.Id,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        IsResistenteSupressores = dto.IsResistenteSupressores
    };

}
