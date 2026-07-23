using Decco.Api.Common;
using Decco.Api.DataLayer.Repositories;
using Decco.Api.DataLayer.Models;
using Decco.Contracts;

namespace Decco.Api.Services;

public class CatMecanismoInteracaoService : ICatMecanismoInteracaoService
{
    private readonly ICatMecanismoInteracaoRepository _repo;

    public CatMecanismoInteracaoService(ICatMecanismoInteracaoRepository repo)
    {
        _repo = repo;
    }

    public async Task<SingleResponse<List<CatMecanismoInteracaoDto>>> List()
    {
        try
        {
            var list = await _repo.ListAsync();
            var dtos = list.Select(MapToDto).ToList();
            return new SingleResponse<List<CatMecanismoInteracaoDto>> { Data = dtos };
        }
        catch
        {
            return ErrorResponseHelper.Fail<List<CatMecanismoInteracaoDto>>();
        }
    }

    public async Task<SingleResponse<CatMecanismoInteracaoDto>> Get(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return ErrorResponseHelper.NotFound<CatMecanismoInteracaoDto>();

            return new SingleResponse<CatMecanismoInteracaoDto> { Data = MapToDto(entity) };
        }
        catch
        {
            return ErrorResponseHelper.Fail<CatMecanismoInteracaoDto>();
        }
    }

    public async Task<SingleResponse<int>> Insert(CatMecanismoInteracaoDto dto)
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

    public async Task<SingleResponse<bool>> Update(CatMecanismoInteracaoDto dto)
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

    private static CatMecanismoInteracaoDto MapToDto(CatMecanismoInteracao e) => new()
    {
        Id = e.Id,
        Codigo = e.Codigo,
        Nome = e.Nome,
        Descricao = e.Descricao,
        CamadaOntologicaId = e.CamadaOntologicaId,
        EhSubnatureza = e.EhSubnatureza
    };

    private static CatMecanismoInteracao MapToEntity(CatMecanismoInteracaoDto dto) => new()
    {
        Id = dto.Id,
        Codigo = dto.Codigo,
        Nome = dto.Nome,
        Descricao = dto.Descricao,
        CamadaOntologicaId = dto.CamadaOntologicaId,
        EhSubnatureza = dto.EhSubnatureza
    };

}
