using Decco.Api.Contracts;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatCamadaOntologicaService : ICatCamadaOntologicaService
{
    private readonly ICatCamadaOntologicaRepository _repo;

    public CatCamadaOntologicaService(ICatCamadaOntologicaRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatCamadaOntologicaDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatCamadaOntologicaDto>> { Data = dtos };
        }
        catch (Exception ex)
        {
            return ErrorResponse<List<CatCamadaOntologicaDto>>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<CatCamadaOntologicaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponse<CatCamadaOntologicaDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);

            return new SingleResponse<CatCamadaOntologicaDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponse<CatCamadaOntologicaDto>(ErrorCodes.InternalError.GetCode(), ex.Message);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatCamadaOntologicaDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatCamadaOntologicaDto dto)
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

    private static CatCamadaOntologicaDto MapToDto(CatCamadaOntologica e) => new()
    {
        Id = e.Id,
        Simbolo = e.Simbolo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        ForcaFundamentalId = e.ForcaFundamentalId,
        Prioridade = e.Prioridade
    };

    private static CatCamadaOntologica MapToEntity(CatCamadaOntologicaDto dto) => new()
    {
        Id = dto.Id,
        Simbolo = dto.Simbolo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        ForcaFundamentalId = dto.ForcaFundamentalId,
        Prioridade = dto.Prioridade
    };

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };
}
