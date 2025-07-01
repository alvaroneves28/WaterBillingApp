using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

public class ConsumptionRepository : IConsumptionRepository
{
    private readonly ApplicationDbContext _context;

    public ConsumptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Consumption>> GetAllAsync()
    {
        return await _context.Consumptions
                             .Include(c => c.Meter)
                             .ToListAsync();
    }

    public async Task<Consumption> GetByIdAsync(int id)
    {
        return await _context.Consumptions
                             .Include(c => c.Meter)
                             .Include(c => c.TariffBracket)
                             .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId)
    {
        return await _context.Consumptions
                             .Where(c => c.MeterId == meterId)
                             .ToListAsync();
    }

    public async Task AddAsync(Consumption consumption)
    {
        _context.Consumptions.Add(consumption);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Consumption consumption)
    {
        _context.Consumptions.Update(consumption);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var consumption = await GetByIdAsync(id);
        if (consumption != null)
        {
            _context.Consumptions.Remove(consumption);
            await _context.SaveChangesAsync();
        }
    }
}
