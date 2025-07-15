using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

public class MeterRepository : GenericRepository<Meter>, IMeterRepository
{
    public MeterRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Meter>> GetPendingMetersAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Where(m => m.Status == MeterStatus.Pending)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meter>> GetActiveMetersAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .Where(m => m.IsActive && m.Status == MeterStatus.Approved)
            .ToListAsync();
    }

    public async Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(m => m.Consumptions)
            .Where(m => m.CustomerId == customerId && m.IsActive)
            .ToListAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public override async Task<Meter> GetByIdAsync(int id)
    {
        var meter = await _dbSet
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meter == null)
            throw new KeyNotFoundException($"Meter with Id {id} not found.");

        return meter;
    }

    public override async Task<IEnumerable<Meter>> GetAllAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .ToListAsync();
    }

    public override async Task DeleteAsync(int id)
    {
        var meter = await _dbSet.FindAsync(id);
        if (meter == null)
            throw new KeyNotFoundException("Meter not found.");

        _dbSet.Remove(meter);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Unable to delete the meter due to existing dependencies.");
        }
    }
}
