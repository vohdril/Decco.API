using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatTipoInstalacaoService : IService
{
    Task<SingleResponse<List<CatTipoInstalacaoDto>>> List();
    Task<SingleResponse<CatTipoInstalacaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatTipoInstalacaoDto dto);
    Task<SingleResponse<bool>> Update(CatTipoInstalacaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
