using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

/// <summary>
/// Operações — o trabalho escopado por instalação.
///
/// O recorte por INSTALAÇÃO PERMITIDA ao usuário ainda não acontece aqui: a
/// relação usuário↔instalação vai viver no DeccoAuthDB, que não existe ainda.
/// Hoje o filtro chega pronto no contrato (OperacaoFiltroDto.InstalacaoId /
/// NivelAcessoUsuario); quando o auth existir, é a Foundation.API que preenche
/// esses campos a partir do usuário — não o cliente.
/// </summary>
public class OperacaoService : IOperacaoService
{
    private const int TamanhoMaximoPagina = 200;

    private readonly IOperacaoRepository _repo;
    private readonly ILogger<OperacaoService> _logger;

    public OperacaoService(IOperacaoRepository repo, ILogger<OperacaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedResponse<OperacaoDto>> List(OperacaoFiltroDto filtro)
    {
        try
        {
            var pageIndex = Math.Max(0, filtro.PageIndex);
            var pageSize = Math.Clamp(filtro.PageSize, 1, TamanhoMaximoPagina);

            var (itens, total) = await _repo.BuscarAsync(new OperacaoFiltro(
                filtro.InstalacaoId,
                filtro.IncluirSubinstalacoes,
                filtro.TipoOperacaoId,
                filtro.Status,
                filtro.AnomaliaId,
                filtro.NivelAcessoUsuario,
                pageIndex,
                pageSize));

            return new PagedResponse<OperacaoDto>
            {
                Data = itens.Select(MapToDto).ToList(),
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

    private static OperacaoDto MapToDto(OperacaoResumo r) => new()
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
