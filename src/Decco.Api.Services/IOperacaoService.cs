using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface IOperacaoService : IService
{
    Task<PagedResponse<OperacaoDto>> List(OperacaoFilterDto filter);
    Task<SingleResponse<OperacaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(OperacaoDto dto);
    Task<SingleResponse<bool>> Update(OperacaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
