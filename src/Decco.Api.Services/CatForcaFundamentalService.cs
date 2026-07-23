using Decco.Api.Common;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatForcaFundamentalService : ICatForcaFundamentalService
{
    private readonly ICatForcaFundamentalRepository _repo;

    public CatForcaFundamentalService(ICatForcaFundamentalRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatForcaFundamentalDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatForcaFundamentalDto>> { Data = dtos };
        }
        catch
        {
            return ErrorResponseHelper.Fail<List<CatForcaFundamentalDto>>();
        }
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
        catch
        {
            return ErrorResponseHelper.Fail<CatForcaFundamentalDto>();
        }
    }

    public async Task<SingleResponse<int>> Insert(CatForcaFundamentalDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatForcaFundamentalDto dto)
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
