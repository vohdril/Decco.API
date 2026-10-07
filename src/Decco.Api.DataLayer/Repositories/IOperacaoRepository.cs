using Decco.Api.Common;
using Decco.Api.DataLayer.Models;

namespace Decco.Api.DataLayer.Repositories;

public interface IOperacaoRepository : IRepository
{
    Task<(List<OperacaoResumo> Itens, int Total)> BuscarAsync(OperacaoFiltro filtro);
    Task<Operacao?> GetByIdAsync(int id);
    Task<int> InsertAsync(Operacao operacao);
    Task UpdateAsync(Operacao operacao);
    Task DeleteAsync(int id);
}
