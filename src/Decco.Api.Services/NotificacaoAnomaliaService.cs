using Decco.Api.Common;
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
        catch { return ErrorResponseHelper.Fail<List<NotificacaoAnomaliaDto>>(); }
    }

    public async Task<SingleResponse<NotificacaoAnomaliaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponseHelper.NotFound<NotificacaoAnomaliaDto>();
            return new SingleResponse<NotificacaoAnomaliaDto> { Data = MapToDto(entity) };
        }
        catch { return ErrorResponseHelper.Fail<NotificacaoAnomaliaDto>(); }
    }

    public async Task<SingleResponse<int>> Insert(NotificacaoAnomaliaDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch { return ErrorResponseHelper.Fail<int>(); }
    }

    public async Task<SingleResponse<bool>> Update(NotificacaoAnomaliaDto dto)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) return ErrorResponseHelper.NotFound<bool>();
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch { return ErrorResponseHelper.Fail<bool>(); }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return ErrorResponseHelper.NotFound<bool>();
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch { return ErrorResponseHelper.Fail<bool>(); }
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

}
