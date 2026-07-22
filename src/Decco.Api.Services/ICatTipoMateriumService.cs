using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatTipoMateriumService : IService
{
    Task<SingleResponse<List<CatTipoMateriumDto>>> List();
    Task<SingleResponse<CatTipoMateriumDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatTipoMateriumDto dto);
    Task<SingleResponse<bool>> Update(CatTipoMateriumDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
