using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatCognicaoAparenteService : IService
{
    Task<SingleResponse<List<CatCognicaoAparenteDto>>> List();
    Task<SingleResponse<CatCognicaoAparenteDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatCognicaoAparenteDto dto);
    Task<SingleResponse<bool>> Update(CatCognicaoAparenteDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
