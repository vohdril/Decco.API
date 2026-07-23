using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface IAnomaliaService : IService
{
    Task<SingleResponse<AnomaliaDto>> Get(int id);
    Task<PagedResponse<AnomaliaDto>> List(int page = 0, int pageSize = 50);
    Task<SingleResponse<int>> Insert(AnomaliaDto dto);
    Task<SingleResponse<bool>> Update(AnomaliaDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
