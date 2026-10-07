using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatTipoInstalacaoRepository : IRepository
{
    Task<List<CatTipoInstalacao>> ListAsync();
    Task<CatTipoInstalacao?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatTipoInstalacao entity);
    Task UpdateAsync(CatTipoInstalacao entity);
    Task DeleteAsync(int id);
}
