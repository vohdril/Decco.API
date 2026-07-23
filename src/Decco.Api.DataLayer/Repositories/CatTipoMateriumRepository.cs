using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatTipoMateriumRepository : ICatTipoMateriumRepository
{
    private readonly DeccoDbContext _context;

    public CatTipoMateriumRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatTipoMaterium>> ListAsync()
    {
        return await _context.Set<CatTipoMaterium>().OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatTipoMaterium?> GetByIdAsync(int id)
    {
        return await _context.Set<CatTipoMaterium>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatTipoMaterium entity)
    {
        _context.Set<CatTipoMaterium>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatTipoMaterium entity)
    {
        _context.Set<CatTipoMaterium>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatTipoMaterium>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatTipoMaterium>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
