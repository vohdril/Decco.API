using Decco.Api.DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace Decco.Api.DataLayer.Repositories;

public class CatMecanismoInteracaoRepository : ICatMecanismoInteracaoRepository
{
    private readonly DeccoDbContext _context;

    public CatMecanismoInteracaoRepository(DeccoDbContext context)
    {
        _context = context;
    }

    public async Task<List<CatMecanismoInteracao>> ListAsync()
    {
        return await _context.CatMecanismoInteracaos.OrderBy(e => e.Nome).ToListAsync();
    }

    public async Task<CatMecanismoInteracao?> GetByIdAsync(int id)
    {
        return await _context.CatMecanismoInteracaos.FindAsync(id);
    }

    public async Task<int> InsertAsync(CatMecanismoInteracao entity)
    {
        _context.CatMecanismoInteracaos.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task UpdateAsync(CatMecanismoInteracao entity)
    {
        _context.CatMecanismoInteracaos.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.CatMecanismoInteracaos.FindAsync(id);
        if (entity != null)
        {
            _context.CatMecanismoInteracaos.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
