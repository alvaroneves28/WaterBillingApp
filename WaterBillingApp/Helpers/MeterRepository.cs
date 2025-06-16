using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

public class MeterRepository : IMeterRepository
{
    private readonly ApplicationDbContext _context;

    public MeterRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Meter meter)
    {
        await _context.Meters.AddAsync(meter);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Meter>> GetAllAsync()
    {
        return await _context.Meters
        .Include(m => m.Customer)
        .ToListAsync();
    }

    public async Task<Meter> GetByIdAsync(int id)
    {
        return await _context.Meters
        .Include(m => m.Customer)
        .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task UpdateAsync(Meter meter)
    {
        _context.Meters.Update(meter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var meter = await _context.Meters.FindAsync(id);
        if (meter != null)
        {
            _context.Meters.Remove(meter);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Meter>> GetPendingMetersAsync()
    {
        return await _context.Meters
            .Include(m => m.Customer)
            .Where(m => m.Status == MeterStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meter>> GetActiveMetersAsync()
    {
        return await _context.Meters
                             .Include(m => m.Customer)
                             .Where(m => m.IsActive)
                             .ToListAsync();
    }

}
