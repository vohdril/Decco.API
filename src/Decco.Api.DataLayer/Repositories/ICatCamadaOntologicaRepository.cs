using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatCamadaOntologicaRepository : IRepository
{
    Task<List<CatCamadaOntologica>> ListAsync();
    Task<CatCamadaOntologica?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatCamadaOntologica entity);
    Task UpdateAsync(CatCamadaOntologica entity);
    Task DeleteAsync(int id);
}
