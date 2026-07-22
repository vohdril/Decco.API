using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface ILaboratorioService : IService
{
    Task<SingleResponse<List<LaboratorioDto>>> List();
    Task<SingleResponse<LaboratorioDto>> Get(int id);
    Task<SingleResponse<int>> Insert(LaboratorioDto dto);
    Task<SingleResponse<bool>> Update(LaboratorioDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
