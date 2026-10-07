using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatOperacaoRepository : ICatOperacaoRepository
{
    private readonly DeccoDbContext _context;

    public CatOperacaoRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatOperacao>> ListAsync()
    {
        return await _context.Set<CatOperacao>().OrderBy(e => e.NivelAcessoMinimo).ThenBy(e => e.Codigo).ToListAsync();
    }

    public async Task<CatOperacao?> GetByIdAsync(int id)
    {
        return await _context.Set<CatOperacao>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatOperacao entity)
    {
        _context.Set<CatOperacao>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatOperacao entity)
    {
        _context.Set<CatOperacao>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatOperacao>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatOperacao>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
