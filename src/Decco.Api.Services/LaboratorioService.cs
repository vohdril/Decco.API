using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.Contracts;
using Decco.Api.DataLayer.Models;
using Decco.Api.DataLayer.Repositories;
using Decco.Contracts;

namespace Decco.Api.Services;

public class LaboratorioService : ILaboratorioService
{
    private readonly ILaboratorioRepository _repo;
    private readonly ILogger<LaboratorioService> _logger;

    public LaboratorioService(ILaboratorioRepository repo, ILogger<LaboratorioService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<LaboratorioDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<LaboratorioDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<LaboratorioDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<LaboratorioDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return ErrorResponseHelper.NotFound<LaboratorioDto>();
            return new SingleResponse<LaboratorioDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<LaboratorioDto>(_logger, ex); }
    }

    public async Task<SingleResponse<int>> Insert(LaboratorioDto dto)
    {
        try
        {
            var id = await _repo.InsertAsync(MapToEntity(dto));
            return new SingleResponse<int> { Data = id };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<int>(_logger, ex); }
    }

    public async Task<SingleResponse<bool>> Update(LaboratorioDto dto)
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

}
