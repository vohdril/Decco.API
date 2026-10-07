using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatTipoInstalacaoService : ICatTipoInstalacaoService
{
    private readonly ICatTipoInstalacaoRepository _repo;
    private readonly ILogger<CatTipoInstalacaoService> _logger;

    public CatTipoInstalacaoService(ICatTipoInstalacaoRepository repo, ILogger<CatTipoInstalacaoService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatTipoInstalacaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatTipoInstalacaoDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatTipoInstalacaoDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatTipoInstalacaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatTipoInstalacaoDto>();

            return new SingleResponse<CatTipoInstalacaoDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatTipoInstalacaoDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatTipoInstalacaoDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatTipoInstalacaoDto dto)
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

    private static CatTipoInstalacaoDto MapToDto(CatTipoInstalacao e) => new()
    {
        Id = e.Id,
        Codigo = e.Codigo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        PermiteFilhos = e.PermiteFilhos,
        Ativo = e.Ativo
    };

    private static CatTipoInstalacao MapToEntity(CatTipoInstalacaoDto dto) => new()
    {
        Id = dto.Id,
        Codigo = dto.Codigo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        PermiteFilhos = dto.PermiteFilhos,
        Ativo = dto.Ativo
    };

}
