using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatForcaFundamentalRepository : ICatForcaFundamentalRepository
{
    private readonly DeccoDbContext _context;

    public CatForcaFundamentalRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatForcaFundamental>> ListAsync()
    {
        return await _context.Set<CatForcaFundamental>().OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatForcaFundamental?> GetByIdAsync(int id)
    {
        return await _context.Set<CatForcaFundamental>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatForcaFundamental entity)
    {
        _context.Set<CatForcaFundamental>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatForcaFundamental entity)
    {
        _context.Set<CatForcaFundamental>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatForcaFundamental>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatForcaFundamental>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
