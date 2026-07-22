using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatForcaFundamentalService : IService
{
    Task<SingleResponse<List<CatForcaFundamentalDto>>> List();
    Task<SingleResponse<CatForcaFundamentalDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatForcaFundamentalDto dto);
    Task<SingleResponse<bool>> Update(CatForcaFundamentalDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
