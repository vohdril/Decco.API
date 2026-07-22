using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class LaboratorioService : ILaboratorioService
{
    private readonly ILaboratorioRepository _repo;

    public LaboratorioService(ILaboratorioRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<LaboratorioDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<LaboratorioDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponse<List<LaboratorioDto>>(ex); }
    }

    public async Task<SingleResponse<LaboratorioDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponse<LaboratorioDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);
            return new SingleResponse<LaboratorioDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponse<LaboratorioDto>(ex); }
    }

    public async Task<SingleResponse<int>> Insert(LaboratorioDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponse<int>(ex); }
    }

    public async Task<SingleResponse<bool>> Update(LaboratorioDto dto)
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

    private static LaboratorioDto MapToDto(Laboratorio e) => new()
    {
        Id = e.Id, Codigo = e.Codigo, Nome = e.Nome, Descricao = e.Descricao ?? string.Empty,
        Sitio = e.Sitio, Responsavel = e.Responsavel, Especialidade = e.Especialidade,
        NivelAcessoMinimo = e.NivelAcessoMinimo, Status = e.Status ?? "ATIVO",
        DataCriacao = e.DataCriacao, DataAtualizacao = e.DataAtualizacao
    };

    private static Laboratorio MapToEntity(LaboratorioDto dto) => new()
    {
        Id = dto.Id, Codigo = dto.Codigo, Nome = dto.Nome, Descricao = dto.Descricao,
        Sitio = dto.Sitio, Responsavel = dto.Responsavel, Especialidade = dto.Especialidade,
        NivelAcessoMinimo = dto.NivelAcessoMinimo, Status = dto.Status
    };

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };

    private static SingleResponse<T> ErrorResponse<T>(Exception ex) =>
        ErrorResponse<T>(ErrorCodes.InternalError.GetCode(), ErrorCodes.InternalError.DefaultMessage);
}
