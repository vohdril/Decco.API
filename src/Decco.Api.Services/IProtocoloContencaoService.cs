using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface IProtocoloContencaoService : IService
{
    Task<SingleResponse<List<ProtocoloContencaoDto>>> List();
    Task<SingleResponse<ProtocoloContencaoDto>> Get(int id);
    Task<SingleResponse<int>> Insert(ProtocoloContencaoDto dto);
    Task<SingleResponse<bool>> Update(ProtocoloContencaoDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
