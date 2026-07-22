using Decco.Api.Contracts;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatPericulosidadeService : ICatPericulosidadeService
{
    private readonly ICatPericulosidadeRepository _repo;

    public CatPericulosidadeService(ICatPericulosidadeRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatPericulosidadeDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatPericulosidadeDto>> { Data = dtos };
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<CatPericulosidadeDto>>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<CatPericulosidadeDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponse<CatPericulosidadeDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);

            return new SingleResponse<CatPericulosidadeDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponse<CatPericulosidadeDto>(ErrorCodes.InternalError.GetCode(), ex.Message);
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
            return ErrorResponse<int>(ErrorCodes.InternalError.GetCode(), ex.Message);
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

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };
}
