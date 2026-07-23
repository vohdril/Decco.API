using Decco.Api.Common;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatCamadaOntologicaService : ICatCamadaOntologicaService
{
    private readonly ICatCamadaOntologicaRepository _repo;

    public CatCamadaOntologicaService(ICatCamadaOntologicaRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatCamadaOntologicaDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatCamadaOntologicaDto>> { Data = dtos };
        }
        catch
        {
            return ErrorResponseHelper.Fail<List<CatCamadaOntologicaDto>>();
        }
    }

    public async Task<SingleResponse<CatCamadaOntologicaDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatCamadaOntologicaDto>();

            return new SingleResponse<CatCamadaOntologicaDto> { Data = MapToDto(entity) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<CatCamadaOntologicaDto>();
        }
    }

    public async Task<SingleResponse<int>> Insert(CatCamadaOntologicaDto dto)
    {
        try
        {
            return new SingleResponse<int> { Data = await _repo.InsertAsync(MapToEntity(dto)) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<int>();
        }
    }

    public async Task<SingleResponse<bool>> Update(CatCamadaOntologicaDto dto)
    {
        try
        {
            await _repo.UpdateAsync(MapToEntity(dto));
            return new SingleResponse<bool> { Data = true };
        }
        catch
        {
            return ErrorResponseHelper.Fail<bool>();
        }
    }

    public async Task<SingleResponse<bool>> Delete(int id)
    {
        try
        {
            await _repo.DeleteAsync(id);
            return new SingleResponse<bool> { Data = true };
        }
        catch
        {
            return ErrorResponseHelper.Fail<bool>();
        }
    }

    private static CatCamadaOntologicaDto MapToDto(CatCamadaOntologica e) => new()
    {
        Id = e.Id,
        Simbolo = e.Simbolo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        ForcaFundamentalId = e.ForcaFundamentalId,
        Prioridade = e.Prioridade
    };

    private static CatCamadaOntologica MapToEntity(CatCamadaOntologicaDto dto) => new()
    {
        Id = dto.Id,
        Simbolo = dto.Simbolo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        ForcaFundamentalId = dto.ForcaFundamentalId,
        Prioridade = dto.Prioridade
    };

}
