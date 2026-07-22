using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IAnomaliaRepository
{
    Task<List<Anomalium>> ListAsync();
    Task<Anomalium?> GetByIdAsync(int id);
    Task<int> InsertAsync(Anomalium anomalia);
    Task UpdateAsync(Anomalium anomalia);
    Task DeleteAsync(int id);
}
