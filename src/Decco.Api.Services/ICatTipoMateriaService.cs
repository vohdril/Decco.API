using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatTipoMateriaService : IService
{
    Task<SingleResponse<List<CatTipoMateriaDto>>> List();
    Task<SingleResponse<CatTipoMateriaDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatTipoMateriaDto dto);
    Task<SingleResponse<bool>> Update(CatTipoMateriaDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
