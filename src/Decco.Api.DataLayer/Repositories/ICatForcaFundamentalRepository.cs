using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatForcaFundamentalRepository : IRepository
{
    Task<List<CatForcaFundamental>> ListAsync();
    Task<CatForcaFundamental?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatForcaFundamental entity);
    Task UpdateAsync(CatForcaFundamental entity);
    Task DeleteAsync(int id);
}
