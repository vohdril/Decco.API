using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface INotificacaoAnomaliaRepository : IRepository
{
    Task<List<NotificacaoAnomalia>> ListAsync();
    Task<NotificacaoAnomalia?> GetByIdAsync(int id);
    Task<int> InsertAsync(NotificacaoAnomalia notificacao);
    Task UpdateAsync(NotificacaoAnomalia notificacao);
    Task DeleteAsync(int id);
}
