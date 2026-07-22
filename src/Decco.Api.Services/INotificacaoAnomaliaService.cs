using Decco.Api.Common;
using Decco.Contracts;

namespace Decco.Api.Services;

public interface INotificacaoAnomaliaService : IService
{
    Task<SingleResponse<List<NotificacaoAnomaliaDto>>> List();
    Task<SingleResponse<NotificacaoAnomaliaDto>> Get(int id);
    Task<SingleResponse<int>> Insert(NotificacaoAnomaliaDto dto);
    Task<SingleResponse<bool>> Update(NotificacaoAnomaliaDto dto);
    Task<SingleResponse<bool>> Delete(int id);
}
