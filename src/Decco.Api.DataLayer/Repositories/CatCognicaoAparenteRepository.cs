using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatCognicaoAparenteRepository : ICatCognicaoAparenteRepository
{
    private readonly DeccoDbContext _context;

    public CatCognicaoAparenteRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatCognicaoAparente>> ListAsync()
    {
        return await _context.Set<CatCognicaoAparente>().OrderBy(e => e.Codigo).ToListAsync();
    }

    public async Task<CatCognicaoAparente?> GetByIdAsync(int id)
    {
        return await _context.Set<CatCognicaoAparente>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatCognicaoAparente entity)
    {
        _context.Set<CatCognicaoAparente>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatCognicaoAparente entity)
    {
        _context.Set<CatCognicaoAparente>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatCognicaoAparente>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatCognicaoAparente>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
