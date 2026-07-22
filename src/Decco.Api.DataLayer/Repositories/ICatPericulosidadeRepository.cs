using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatPericulosidadeRepository : IRepository
{
    Task<List<CatPericulosidade>> ListAsync();
    Task<CatPericulosidade?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatPericulosidade entity);
    Task UpdateAsync(CatPericulosidade entity);
    Task DeleteAsync(int id);
}
