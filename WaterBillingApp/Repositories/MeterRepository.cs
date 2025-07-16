using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

public class MeterRepository : GenericRepository<Meter>, IMeterRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MeterRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public MeterRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves all meters with a status of pending, including related customer data.
    /// </summary>
    /// <returns>A collection of pending meters.</returns>
    public async Task<IEnumerable<Meter>> GetPendingMetersAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Where(m => m.Status == MeterStatus.Pending)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all active and approved meters including related customer and consumption data.
    /// </summary>
    /// <returns>A collection of active meters.</returns>
    public async Task<IEnumerable<Meter>> GetActiveMetersAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .Where(m => m.IsActive && m.Status == MeterStatus.Approved)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves all active meters for a specified customer, including consumption data.
    /// </summary>
    /// <param name="customerId">The ID of the customer.</param>
    /// <returns>A collection of meters for the customer.</returns>
    public async Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(m => m.Consumptions)
            .Where(m => m.CustomerId == customerId && m.IsActive)
            .ToListAsync();
    }

    /// <summary>
    /// Saves any pending changes to the database context.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a meter by its ID, including customer and consumption details.
    /// </summary>
    /// <param name="id">The meter ID.</param>
    /// <returns>The meter entity.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the meter is not found.</exception>
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

    /// <summary>
    /// Retrieves all meters, including related customer and consumption details.
    /// </summary>
    /// <returns>A collection of all meters.</returns>
    public override async Task<IEnumerable<Meter>> GetAllAsync()
    {
        return await _dbSet
            .Include(m => m.Customer)
            .Include(m => m.Consumptions)
            .ToListAsync();
    }

    /// <summary>
    /// Deletes a meter by its ID.
    /// </summary>
    /// <param name="id">The ID of the meter to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the meter is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the meter cannot be deleted due to existing dependencies.</exception>
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
