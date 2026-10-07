using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IAnomaliaRepository : IRepository
{
    Task<(List<Anomalia> Itens, int Total)> ListAsync(int pageIndex, int pageSize);
    Task<Anomalia?> GetByIdAsync(int id);
    Task<int> InsertAsync(Anomalia anomalia);
    Task UpdateAsync(Anomalia anomalia);
    Task DeleteAsync(int id);
}
