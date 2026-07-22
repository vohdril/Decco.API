using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class ProtocoloContencaoService : IProtocoloContencaoService
{
    private readonly IProtocoloContencaoRepository _repo;

    public ProtocoloContencaoService(IProtocoloContencaoRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<ProtocoloContencaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<ProtocoloContencaoDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponse<List<ProtocoloContencaoDto>>(ex); }
    }

    public async Task<SingleResponse<ProtocoloContencaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponse<ProtocoloContencaoDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);
            return new SingleResponse<ProtocoloContencaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponse<ProtocoloContencaoDto>(ex); }
    }

    public async Task<SingleResponse<int>> Insert(ProtocoloContencaoDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponse<int>(ex); }
    }

    public async Task<SingleResponse<bool>> Update(ProtocoloContencaoDto dto)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) return ErrorResponse<bool>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex) { return ErrorResponse<bool>(ex); }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return ErrorResponse<bool>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex) { return ErrorResponse<bool>(ex); }
    }

    private static ProtocoloContencaoDto MapToDto(ProtocoloContencao e) => new()
    {
        Id = e.Id, Codigo = e.Codigo, Titulo = e.Titulo, Descricao = e.Descricao,
        NivelUrgencia = e.NivelUrgencia, ClassesAplicaveis = e.ClassesAplicaveis,
        Passos = e.Passos, RecursosNecessarios = e.RecursosNecessarios,
        DataCriacao = e.DataCriacao, DataAtualizacao = e.DataAtualizacao
    };

    private static ProtocoloContencao MapToEntity(ProtocoloContencaoDto dto) => new()
    {
        Id = dto.Id, Codigo = dto.Codigo, Titulo = dto.Titulo, Descricao = dto.Descricao,
        NivelUrgencia = dto.NivelUrgencia, ClassesAplicaveis = dto.ClassesAplicaveis,
        Passos = dto.Passos, RecursosNecessarios = dto.RecursosNecessarios
    };

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };

    private static SingleResponse<T> ErrorResponse<T>(Exception ex) =>
        ErrorResponse<T>(ErrorCodes.InternalError.GetCode(), ErrorCodes.InternalError.DefaultMessage);
}
