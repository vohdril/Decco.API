using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface ILaboratorioRepository : IRepository
{
    Task<List<Laboratorio>> ListAsync();
    Task<Laboratorio?> GetByIdAsync(int id);
    Task<int> InsertAsync(Laboratorio laboratorio);
    Task UpdateAsync(Laboratorio laboratorio);
    Task DeleteAsync(int id);
}
