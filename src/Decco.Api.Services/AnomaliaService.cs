using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class AnomaliaService : IAnomaliaService
{
    private readonly IAnomaliaRepository _repo;
    private readonly ILogger<AnomaliaService> _logger;

    public AnomaliaService(IAnomaliaRepository repo, ILogger<AnomaliaService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<AnomaliaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<AnomaliaDto>();

            return new SingleResponse<AnomaliaDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<AnomaliaDto>(_logger, ex);
        }
    }

    public async Task<PagedResponse<AnomaliaDto>> List(int page = 0, int pageSize = 50)
    {
        try
        {
            page = Math.Max(0, page);
            pageSize = Math.Clamp(pageSize, 1, 200);

            // Paginação no banco (Skip/Take no repositório), não em memória.
            var (entities, total) = await _repo.ListAsync(page, pageSize);
            var items = entities.Select(MapToDto).ToList();

            return new PagedResponse<AnomaliaDto>
            {
                Data = items,
                PageIndex = page,
                PageSize = pageSize,
                TotalRecords = total
            };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.FailPaged<AnomaliaDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(AnomaliaDto dto)
    {
        try
        {
            var entity = MapToEntity(dto);
            var id = await _repo.InsertAsync(entity);
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<int>(_logger, ex);
        }
    }

    public async Task<SingleResponse<bool>> Update(AnomaliaDto dto)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                return ErrorResponseHelper.NotFound<bool>();

            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<bool>(_logger, ex);
        }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return ErrorResponseHelper.NotFound<bool>();

            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<bool>(_logger, ex);
        }
    }

    private static AnomaliaDto MapToDto(Anomalia entity) => new()
    {
        Id = entity.Id,
        CodigoSCP = entity.CodigoScp,
        NomeComum = entity.NomeComum,
        Descricao = entity.Descricao,
        ClasseObjetoId = entity.ClasseObjetoId,
        ClasseObjeto = entity.ClasseObjeto?.Nome ?? string.Empty,
        CamadaOntologicaId = entity.CamadaOntologicaId,
        CamadaOntologica = entity.CamadaOntologica?.Nome ?? string.Empty,
        TipoMateriaId = entity.TipoMateriaId,
        TipoMateria = entity.TipoMateria?.Nome ?? string.Empty,
        CognicaoAparenteId = entity.CognicaoAparenteId,
        CognicaoAparente = entity.CognicaoAparente?.Nome,
        PericulosidadeId = entity.PericulosidadeId,
        Periculosidade = entity.Periculosidade?.Nome,
        MecanismoPrimarioId = entity.MecanismoPrimarioId,
        MecanismoPrimario = entity.MecanismoPrimario?.Nome ?? string.Empty,
        MecanismoSecundarioId = entity.MecanismoSecundarioId,
        MecanismoSecundario = entity.MecanismoSecundario?.Nome,
        IEIA_D_Base = entity.IeiaDBase,
        FatorCoerenciaSpin = entity.FatorCoerenciaSpin,
        Status = entity.Status ?? "ATIVA",
        InstalacaoContencaoId = entity.InstalacaoContencaoId,
        InstalacaoContencaoCodigo = entity.InstalacaoContencao?.Codigo,
        InstalacaoContencao = entity.InstalacaoContencao?.Nome,
        ResponsavelPesquisa = entity.ResponsavelPesquisa,
        DataCriacao = entity.DataCriacao ?? DateTime.Now,
        DataAtualizacao = entity.DataAtualizacao ?? DateTime.Now
    };

    // Os Ids de classificação NÃO eram mapeados (o DTO só tinha os nomes), então
    // todo Insert chegava à SP com ClasseObjetoId/CamadaOntologicaId/... = 0 e
    // falhava na FK. Coberto por AnomaliaServiceTests.
    private static Anomalia MapToEntity(AnomaliaDto dto) => new()
    {
        Id = dto.Id,
        CodigoScp = dto.CodigoSCP,
        NomeComum = dto.NomeComum,
        Descricao = dto.Descricao,
        ClasseObjetoId = dto.ClasseObjetoId,
        CamadaOntologicaId = dto.CamadaOntologicaId,
        TipoMateriaId = dto.TipoMateriaId,
        CognicaoAparenteId = dto.CognicaoAparenteId,
        PericulosidadeId = dto.PericulosidadeId,
        MecanismoPrimarioId = dto.MecanismoPrimarioId,
        MecanismoSecundarioId = dto.MecanismoSecundarioId,
        IeiaDBase = dto.IEIA_D_Base,
        FatorCoerenciaSpin = dto.FatorCoerenciaSpin,
        Status = dto.Status,
        InstalacaoContencaoId = dto.InstalacaoContencaoId,
        ResponsavelPesquisa = dto.ResponsavelPesquisa
    };
}
