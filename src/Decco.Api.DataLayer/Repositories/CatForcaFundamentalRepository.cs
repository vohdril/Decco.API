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
        return await _context.CatForcaFundamentals.OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatForcaFundamental?> GetByIdAsync(int id)
    {
        return await _context.CatForcaFundamentals.FindAsync(id);
    }

    public async Task<int> InsertAsync(CatForcaFundamental entity)
    {
        _context.CatForcaFundamentals.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatForcaFundamental entity)
    {
        _context.CatForcaFundamentals.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.CatForcaFundamentals.FindAsync(id);
        if (entity != null)
        {
            _context.CatForcaFundamentals.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
