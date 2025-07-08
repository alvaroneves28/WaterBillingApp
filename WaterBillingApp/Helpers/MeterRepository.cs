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
        .Include(m => m.Consumptions)
        //.Include(m => m.Invoice)
        .ToListAsync();
    }

    public async Task<Meter> GetByIdAsync(int id)
    {
        var meter = await _context.Meters
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (meter == null)
        {
            throw new KeyNotFoundException($"Meter with Id {id} not found.");
        }

        return meter;
    }

    public async Task UpdateAsync(Meter meter)
    {
        _context.Meters.Update(meter);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var meter = await _context.Meters.FindAsync(id);
        if (meter == null)
            throw new KeyNotFoundException("Meter not found.");

        _context.Meters.Remove(meter);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Unable to delete the meter due to existing dependencies.");
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
        .Include(m => m.Consumptions)
        .Where(m => m.IsActive && m.Status == MeterStatus.Approved)
        .ToListAsync();
    }

    public async Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId)
    {
        return await _context.Meters
            .Include(m => m.Consumptions)
            .Where(m => m.CustomerId == customerId && m.IsActive)
            .ToListAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }



}
