using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatTipoInstalacaoRepository : ICatTipoInstalacaoRepository
{
    private readonly DeccoDbContext _context;

    public CatTipoInstalacaoRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatTipoInstalacao>> ListAsync()
    {
        return await _context.Set<CatTipoInstalacao>().OrderBy(e => e.Codigo).ToListAsync();
    }

    public async Task<CatTipoInstalacao?> GetByIdAsync(int id)
    {
        return await _context.Set<CatTipoInstalacao>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatTipoInstalacao entity)
    {
        _context.Set<CatTipoInstalacao>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatTipoInstalacao entity)
    {
        _context.Set<CatTipoInstalacao>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatTipoInstalacao>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatTipoInstalacao>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
