using Decco.Api.Contracts;
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
        catch (Exception ex)
        {
            return ErrorResponse<List<CatTipoMateriumDto>>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<CatTipoMateriumDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponse<CatTipoMateriumDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);

            return new SingleResponse<CatTipoMateriumDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponse<CatTipoMateriumDto>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatTipoMateriumDto dto)
    {
        try
        {
            return new SingleResponse<int> { Data = await _repo.InsertAsync(MapToEntity(dto)) };
        }
        catch (Exception ex)
        {
            return ErrorResponse<int>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<bool>> Update(CatTipoMateriumDto dto)
    {
        try
        {
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponse<bool>(ErrorCodes.InternalError.GetCode(), ex.Message);
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
            return ErrorResponse<bool>(ErrorCodes.InternalError.GetCode(), ex.Message);
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

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };
}
