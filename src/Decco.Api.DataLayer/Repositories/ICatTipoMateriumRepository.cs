using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatTipoMateriumRepository : IRepository
{
    Task<List<CatTipoMaterium>> ListAsync();
    Task<CatTipoMaterium?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatTipoMaterium entity);
    Task UpdateAsync(CatTipoMaterium entity);
    Task DeleteAsync(int id);
}
