using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatOperacaoService : ICatOperacaoService
{
    private readonly ICatOperacaoRepository _repo;
    private readonly ILogger<CatOperacaoService> _logger;

    public CatOperacaoService(ICatOperacaoRepository repo, ILogger<CatOperacaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatOperacaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatOperacaoDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatOperacaoDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatOperacaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatOperacaoDto>();

            return new SingleResponse<CatOperacaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatOperacaoDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatOperacaoDto dto)
    {
        try
        {
            return new SingleResponse<int> { Data = await _repo.InsertAsync(MapToEntity(dto)) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<int>(_logger, ex);
        }
    }

    public async Task<SingleResponse<bool>> Update(CatOperacaoDto dto)
    {
        try
        {
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
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<bool>(_logger, ex);
        }
    }

    private static CatOperacaoDto MapToDto(CatOperacao e) => new()
    {
        Id = e.Id,
        Codigo = e.Codigo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        RequerAnomalia = e.RequerAnomalia,
        NivelAcessoMinimo = e.NivelAcessoMinimo,
        CorAlerta = e.CorAlerta,
        Ativo = e.Ativo
    };

    private static CatOperacao MapToEntity(CatOperacaoDto dto) => new()
    {
        Id = dto.Id,
        Codigo = dto.Codigo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        RequerAnomalia = dto.RequerAnomalia,
        NivelAcessoMinimo = dto.NivelAcessoMinimo,
        CorAlerta = dto.CorAlerta,
        Ativo = dto.Ativo
    };

}
