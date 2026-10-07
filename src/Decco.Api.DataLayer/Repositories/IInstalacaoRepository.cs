using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IInstalacaoRepository : IRepository
{
    Task<List<Instalacao>> ListAsync();
    Task<Instalacao?> GetByIdAsync(int id);
    Task<int> InsertAsync(Instalacao instalacao);
    Task UpdateAsync(Instalacao instalacao);
    Task DeleteAsync(int id);
}
