using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IProtocoloContencaoRepository : IRepository
{
    Task<List<ProtocoloContencao>> ListAsync();
    Task<ProtocoloContencao?> GetByIdAsync(int id);
    Task<int> InsertAsync(ProtocoloContencao protocolo);
    Task UpdateAsync(ProtocoloContencao protocolo);
    Task DeleteAsync(int id);
}
