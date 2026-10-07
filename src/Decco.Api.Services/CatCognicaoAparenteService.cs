using Decco.Api.Common;
using Microsoft.Extensions.Logging;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatCognicaoAparenteService : ICatCognicaoAparenteService
{
    private readonly ICatCognicaoAparenteRepository _repo;
    private readonly ILogger<CatCognicaoAparenteService> _logger;

    public CatCognicaoAparenteService(ICatCognicaoAparenteRepository repo, ILogger<CatCognicaoAparenteService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SingleResponse<List<CatCognicaoAparenteDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatCognicaoAparenteDto>> { Data = dtos };
        }
        catch (Exception ex) { return ErrorResponseHelper.Fail<List<CatCognicaoAparenteDto>>(_logger, ex); }
    }

    public async Task<SingleResponse<CatCognicaoAparenteDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatCognicaoAparenteDto>();

            return new SingleResponse<CatCognicaoAparenteDto> { Data = MapToDto(entity) };
        }
        catch (Exception ex)
        {
            return ErrorResponseHelper.Fail<CatCognicaoAparenteDto>(_logger, ex);
        }
    }

    public async Task<SingleResponse<int>> Insert(CatCognicaoAparenteDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatCognicaoAparenteDto dto)
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

    private static CatCognicaoAparenteDto MapToDto(CatCognicaoAparente e) => new()
    {
        Id = e.Id,
        Codigo = e.Codigo,
        Nome = e.Nome,
        Descricao = e.Descricao
    };

    private static CatCognicaoAparente MapToEntity(CatCognicaoAparenteDto dto) => new()
    {
        Id = dto.Id,
        Codigo = dto.Codigo,
        Nome = dto.Nome,
        Descricao = dto.Descricao
    };

}
