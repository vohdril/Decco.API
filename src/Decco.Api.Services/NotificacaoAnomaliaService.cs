using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class NotificacaoAnomaliaService : INotificacaoAnomaliaService
{
    private readonly INotificacaoAnomaliaRepository _repo;

    public NotificacaoAnomaliaService(INotificacaoAnomaliaRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<NotificacaoAnomaliaDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<NotificacaoAnomaliaDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponse<List<NotificacaoAnomaliaDto>>(ex); }
    }

    public async Task<SingleResponse<NotificacaoAnomaliaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponse<NotificacaoAnomaliaDto>(ErrorCodes.NotFound.GetCode(), ErrorCodes.NotFound.DefaultMessage);
            return new SingleResponse<NotificacaoAnomaliaDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponse<NotificacaoAnomaliaDto>(ex); }
    }

    public async Task<SingleResponse<int>> Insert(NotificacaoAnomaliaDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponse<int>(ex); }
    }

    public async Task<SingleResponse<bool>> Update(NotificacaoAnomaliaDto dto)
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

    private static NotificacaoAnomaliaDto MapToDto(NotificacaoAnomalia e) => new()
    {
        Id = e.Id, Titulo = e.Titulo, Descricao = e.Descricao,
        LocalIdentificado = e.LocalIdentificado, DataHora = e.DataHora,
        Status = e.Status ?? "PENDENTE", NivelPrioridade = e.NivelPrioridade,
        Relator = e.Relator, AnomaliaId = e.AnomaliaId, DataResolucao = e.DataResolucao
    };

    private static NotificacaoAnomalia MapToEntity(NotificacaoAnomaliaDto dto) => new()
    {
        Id = dto.Id, Titulo = dto.Titulo, Descricao = dto.Descricao,
        LocalIdentificado = dto.LocalIdentificado, Status = dto.Status,
        NivelPrioridade = dto.NivelPrioridade, Relator = dto.Relator, AnomaliaId = dto.AnomaliaId
    };

    private static SingleResponse<T> ErrorResponse<T>(string code, string message) => new()
    {
        Status = ResponseStatus.Fail,
        Error = new ErrorInfo { Code = code, Message = message }
    };

    private static SingleResponse<T> ErrorResponse<T>(Exception ex) =>
        ErrorResponse<T>(ErrorCodes.InternalError.GetCode(), ErrorCodes.InternalError.DefaultMessage);
}
