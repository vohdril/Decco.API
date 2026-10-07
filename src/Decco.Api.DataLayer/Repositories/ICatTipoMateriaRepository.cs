using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatTipoMateriaRepository : IRepository
{
    Task<List<CatTipoMateria>> ListAsync();
    Task<CatTipoMateria?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatTipoMateria entity);
    Task UpdateAsync(CatTipoMateria entity);
    Task DeleteAsync(int id);
}
