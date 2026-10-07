using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

/// <summary>
/// Operations — the facility-scoped work.
///
/// Slicing by the facilities ALLOWED to the user does not happen here yet: the
/// user↔facility relation will live in DeccoAuthDB, which does not exist yet.
/// Today the filter arrives ready in the contract (OperacaoFilterDto.InstalacaoId /
/// NivelAcessoUsuario); once auth exists, Foundation.API fills those fields from the
/// user — not the client.
/// </summary>
public class OperacaoService : IOperacaoService
{
    private const int MaxPageSize = 200;

    private readonly IOperacaoRepository _repo;
    private readonly ILogger<OperacaoService> _logger;

    public OperacaoService(IOperacaoRepository repo, ILogger<OperacaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedResponse<OperacaoDto>> List(OperacaoFilterDto filter)
    {
        try
        {
            var pageIndex = Math.Max(0, filter.PageIndex);
            var pageSize = Math.Clamp(filter.PageSize, 1, MaxPageSize);

            var (items, total) = await _repo.SearchAsync(new OperacaoFilter(
                filter.InstalacaoId,
                filter.IncluirSubinstalacoes,
                filter.TipoOperacaoId,
                filter.Status,
                filter.AnomaliaId,
                filter.NivelAcessoUsuario,
                pageIndex,
                pageSize));

            return new PagedResponse<OperacaoDto>
            {
                Data = items.Select(MapToDto).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalRecords = total
            };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.FailPaged<OperacaoDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<OperacaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponseHelper.NotFound<OperacaoDto>();
            return new SingleResponse<OperacaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<OperacaoDto>(_logger, ex); }
    }

    public async Task<SingleResponse<int>> Insert(OperacaoDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<int>(_logger, ex); }
    }

    public async Task<SingleResponse<bool>> Update(OperacaoDto dto)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) return ErrorResponseHelper.NotFound<bool>();
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<bool>(_logger, ex); }
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
        catch (Exception ex) { return ErrorResponseHelper.Fail<bool>(_logger, ex); }
    }

    private static OperacaoDto MapToDto(OperacaoSummary r) => new()
    {
        Id = r.Id, Codigo = r.Codigo, Codinome = r.Codinome,
        TipoOperacaoId = r.TipoOperacaoId, TipoOperacaoCodigo = r.TipoOperacaoCodigo, TipoOperacao = r.TipoOperacao,
        InstalacaoId = r.InstalacaoId, InstalacaoCodigo = r.InstalacaoCodigo, Instalacao = r.Instalacao,
        AnomaliaId = r.AnomaliaId, AnomaliaCodigo = r.AnomaliaCodigo,
        NotificacaoId = r.NotificacaoId, ProtocoloId = r.ProtocoloId, ProtocoloCodigo = r.ProtocoloCodigo,
        Objetivo = r.Objetivo, Status = r.Status, Prioridade = r.Prioridade,
        NivelAcessoMinimo = r.NivelAcessoMinimo, Responsavel = r.Responsavel,
        DataAbertura = r.DataAbertura, DataPrevisaoTermino = r.DataPrevisaoTermino,
        DataEncerramento = r.DataEncerramento
    };

    private static OperacaoDto MapToDto(Operacao e) => new()
    {
        Id = e.Id, Codigo = e.Codigo, Codinome = e.Codinome,
        TipoOperacaoId = e.TipoOperacaoId, TipoOperacaoCodigo = e.TipoOperacao?.Codigo, TipoOperacao = e.TipoOperacao?.Nome,
        InstalacaoId = e.InstalacaoId, InstalacaoCodigo = e.Instalacao?.Codigo, Instalacao = e.Instalacao?.Nome,
        AnomaliaId = e.AnomaliaId, AnomaliaCodigo = e.Anomalia?.CodigoScp,
        NotificacaoId = e.NotificacaoId, ProtocoloId = e.ProtocoloId, ProtocoloCodigo = e.Protocolo?.Codigo,
        Objetivo = e.Objetivo, Descricao = e.Descricao, Status = e.Status, Prioridade = e.Prioridade,
        NivelAcessoMinimo = e.NivelAcessoMinimo, Responsavel = e.Responsavel,
        DataAbertura = e.DataAbertura, DataPrevisaoTermino = e.DataPrevisaoTermino,
        DataEncerramento = e.DataEncerramento, ResultadoResumo = e.ResultadoResumo,
        DataCriacao = e.DataCriacao, DataAtualizacao = e.DataAtualizacao
    };

    private static Operacao MapToEntity(OperacaoDto dto) => new()
    {
        Id = dto.Id, Codigo = dto.Codigo ?? string.Empty, Codinome = dto.Codinome,
        TipoOperacaoId = dto.TipoOperacaoId, InstalacaoId = dto.InstalacaoId,
        AnomaliaId = dto.AnomaliaId, NotificacaoId = dto.NotificacaoId, ProtocoloId = dto.ProtocoloId,
        Objetivo = dto.Objetivo, Descricao = dto.Descricao, Status = dto.Status,
        Prioridade = dto.Prioridade, NivelAcessoMinimo = dto.NivelAcessoMinimo,
        Responsavel = dto.Responsavel, DataPrevisaoTermino = dto.DataPrevisaoTermino,
        DataEncerramento = dto.DataEncerramento, ResultadoResumo = dto.ResultadoResumo
    };
}
