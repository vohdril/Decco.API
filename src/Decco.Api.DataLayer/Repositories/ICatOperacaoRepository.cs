using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatOperacaoRepository : IRepository
{
    Task<List<CatOperacao>> ListAsync();
    Task<CatOperacao?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatOperacao entity);
    Task UpdateAsync(CatOperacao entity);
    Task DeleteAsync(int id);
}
