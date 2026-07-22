using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatClasseObjetoRepository : ICatClasseObjetoRepository
{
    private readonly DeccoDbContext _context;

    public CatClasseObjetoRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatClasseObjeto>> ListAsync()
    {
        return await _context.CatClasseObjetos.OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatClasseObjeto?> GetByIdAsync(int id)
    {
        return await _context.CatClasseObjetos.FindAsync(id);
    }

    public async Task<int> InsertAsync(CatClasseObjeto entity)
    {
        _context.CatClasseObjetos.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatClasseObjeto entity)
    {
        _context.CatClasseObjetos.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.CatClasseObjetos.FindAsync(id);
        if (entity != null)
        {
            _context.CatClasseObjetos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
