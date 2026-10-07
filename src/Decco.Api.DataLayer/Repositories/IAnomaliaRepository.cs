using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IAnomaliaRepository : IRepository
{
    Task<List<Anomalia>> ListAsync();
    Task<Anomalia?> GetByIdAsync(int id);
    Task<int> InsertAsync(Anomalia anomalia);
    Task UpdateAsync(Anomalia anomalia);
    Task DeleteAsync(int id);
}
