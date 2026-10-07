using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class InstalacaoService : IInstalacaoService
{
    private readonly IInstalacaoRepository _repo;
    private readonly ILogger<InstalacaoService> _logger;

    public InstalacaoService(IInstalacaoRepository repo, ILogger<InstalacaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<InstalacaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<InstalacaoDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<InstalacaoDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<InstalacaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponseHelper.NotFound<InstalacaoDto>();
            return new SingleResponse<InstalacaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<InstalacaoDto>(_logger, ex); }
    }

    public async Task<SingleResponse<int>> Insert(InstalacaoDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<int>(_logger, ex); }
    }

    public async Task<SingleResponse<bool>> Update(InstalacaoDto dto)
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

    private static InstalacaoDto MapToDto(Instalacao e) => new()
    {
        Id = e.Id, Codigo = e.Codigo, Nome = e.Nome, Descricao = e.Descricao,
        TipoInstalacaoId = e.TipoInstalacaoId,
        TipoInstalacaoCodigo = e.TipoInstalacao?.Codigo, TipoInstalacao = e.TipoInstalacao?.Nome,
        InstalacaoPaiId = e.InstalacaoPaiId, InstalacaoPaiCodigo = e.InstalacaoPai?.Codigo,
        Responsavel = e.Responsavel, Especialidade = e.Especialidade,
        NivelAcessoMinimo = e.NivelAcessoMinimo, Status = e.Status,
        DataCriacao = e.DataCriacao, DataAtualizacao = e.DataAtualizacao
    };

    private static Instalacao MapToEntity(InstalacaoDto dto) => new()
    {
        Id = dto.Id, Codigo = dto.Codigo, Nome = dto.Nome, Descricao = dto.Descricao,
        TipoInstalacaoId = dto.TipoInstalacaoId, InstalacaoPaiId = dto.InstalacaoPaiId,
        Responsavel = dto.Responsavel, Especialidade = dto.Especialidade,
        NivelAcessoMinimo = dto.NivelAcessoMinimo, Status = dto.Status
    };

}
