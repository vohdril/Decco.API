using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface IAnomaliaService : IService
{
    Task<SingleResponse<AnomaliaDto>> GetAnomalia(int id);
    Task<PagedResponse<AnomaliaDto>> ListAnomalias(int page = 0, int pageSize = 50);
    Task<SingleResponse<int>> InsertAnomalia(AnomaliaDto dto);
    Task<SingleResponse<bool>> UpdateAnomalia(AnomaliaDto dto);
    Task<SingleResponse<bool>> DeleteAnomalia(int id);
}
