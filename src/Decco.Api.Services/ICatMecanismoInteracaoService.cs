using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ICatMecanismoInteracaoService : IService
{
    Task<SingleResponse<List<CatMecanismoInteracaoDto>>> List();
    Task<SingleResponse<CatMecanismoInteracaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(CatMecanismoInteracaoDto dto);
    Task<SingleResponse<bool>> Update(CatMecanismoInteracaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
