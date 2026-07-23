using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatPericulosidadeRepository : ICatPericulosidadeRepository
{
    private readonly DeccoDbContext _context;

    public CatPericulosidadeRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatPericulosidade>> ListAsync()
    {
        return await _context.Set<CatPericulosidade>().OrderBy(e => e.Nivel).ToListAsync();
    }

    public async Task<CatPericulosidade?> GetByIdAsync(int id)
    {
        return await _context.Set<CatPericulosidade>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatPericulosidade entity)
    {
        _context.Set<CatPericulosidade>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatPericulosidade entity)
    {
        _context.Set<CatPericulosidade>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatPericulosidade>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatPericulosidade>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
