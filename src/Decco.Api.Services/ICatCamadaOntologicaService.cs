using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatCamadaOntologicaService : IService
{
    Task<SingleResponse<List<CatCamadaOntologicaDto>>> List();
    Task<SingleResponse<CatCamadaOntologicaDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatCamadaOntologicaDto dto);
    Task<SingleResponse<bool>> Update(CatCamadaOntologicaDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
