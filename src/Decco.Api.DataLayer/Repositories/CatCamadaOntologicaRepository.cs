using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatCamadaOntologicaRepository : ICatCamadaOntologicaRepository
{
    private readonly DeccoDbContext _context;

    public CatCamadaOntologicaRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatCamadaOntologica>> ListAsync()
    {
        return await _context.Set<CatCamadaOntologica>().OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatCamadaOntologica?> GetByIdAsync(int id)
    {
        return await _context.Set<CatCamadaOntologica>().FindAsync(id);
    }

    public async Task<int> InsertAsync(CatCamadaOntologica entity)
    {
        _context.Set<CatCamadaOntologica>().Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatCamadaOntologica entity)
    {
        _context.Set<CatCamadaOntologica>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Set<CatCamadaOntologica>().FindAsync(id);
        if (entity != null)
        {
            _context.Set<CatCamadaOntologica>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
