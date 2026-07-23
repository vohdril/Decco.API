using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ICatMecanismoInteracaoRepository : IRepository
{
    Task<List<CatMecanismoInteracao>> ListAsync();
    Task<CatMecanismoInteracao?> GetByIdAsync(int id);
    Task<int> InsertAsync(CatMecanismoInteracao entity);
    Task UpdateAsync(CatMecanismoInteracao entity);
    Task DeleteAsync(int id);
}
