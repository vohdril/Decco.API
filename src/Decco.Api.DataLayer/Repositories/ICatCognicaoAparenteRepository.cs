using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatCognicaoAparenteRepository : IRepository
{
    Task<List<CatCognicaoAparente>> ListAsync();
    Task<CatCognicaoAparente?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatCognicaoAparente entity);
    Task UpdateAsync(CatCognicaoAparente entity);
    Task DeleteAsync(int id);
}
