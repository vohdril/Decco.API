using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatOperacaoService : IService
{
    Task<SingleResponse<List<CatOperacaoDto>>> List();
    Task<SingleResponse<CatOperacaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatOperacaoDto dto);
    Task<SingleResponse<bool>> Update(CatOperacaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
