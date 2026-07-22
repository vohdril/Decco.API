using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatPericulosidadeService : IService
{
    Task<SingleResponse<List<CatPericulosidadeDto>>> List();
    Task<SingleResponse<CatPericulosidadeDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatPericulosidadeDto dto);
    Task<SingleResponse<bool>> Update(CatPericulosidadeDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
