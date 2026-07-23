using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatClasseObjetoRepository : IRepository
{
    Task<List<CatClasseObjeto>> ListAsync();
    Task<CatClasseObjeto?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatClasseObjeto entity);
    Task UpdateAsync(CatClasseObjeto entity);
    Task DeleteAsync(int id);
}
