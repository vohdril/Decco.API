using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface IInstalacaoService : IService
{
    Task<SingleResponse<List<InstalacaoDto>>> List();
    Task<SingleResponse<InstalacaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(InstalacaoDto dto);
    Task<SingleResponse<bool>> Update(InstalacaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
