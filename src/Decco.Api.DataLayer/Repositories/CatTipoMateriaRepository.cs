using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatTipoMateriaRepository : ICatTipoMateriaRepository
{
    private readonly DeccoDbContext _context;

    public CatTipoMateriaRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatTipoMateria>> ListAsync()
    {
        return await _context.Set<CatTipoMateria>().OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatTipoMateria?> GetByIdAsync(int id)
    {
        return await _context.Set<CatTipoMateria>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatTipoMateria entity)
    {
        _context.Set<CatTipoMateria>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatTipoMateria entity)
    {
        _context.Set<CatTipoMateria>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatTipoMateria>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatTipoMateria>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
