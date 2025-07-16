using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

/// <summary>
/// Repository for managing <see cref="Consumption"/> entities.
/// Provides methods for querying and manipulating water consumption data.
/// </summary>
public class ConsumptionRepository : GenericRepository<Consumption>, IConsumptionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumptionRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public ConsumptionRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Retrieves all consumption records associated with a specific meter.
    /// </summary>
    /// <param name="meterId">The ID of the meter.</param>
    /// <returns>A collection of <see cref="Consumption"/> records for the specified meter.</returns>
    public async Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId)
    {
        return await _dbSet
            .Where(c => c.MeterId == meterId)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if there are any consumption records for the specified meter.
    /// </summary>
    /// <param name="meterId">The ID of the meter.</param>
    /// <returns><c>true</c> if there is at least one consumption record; otherwise, <c>false</c>.</returns>
    public async Task<bool> AnyByMeterIdAsync(int meterId)
    {
        return await _dbSet.AnyAsync(c => c.MeterId == meterId);
    }

    /// <summary>
    /// Retrieves all consumption records, including related meter data.
    /// </summary>
    /// <returns>A collection of all <see cref="Consumption"/> records with meter information.</returns>
    public override async Task<IEnumerable<Consumption>> GetAllAsync()
    {
        return await _dbSet
            .Include(c => c.Meter)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a consumption record by its ID, including related meter and tariff bracket data.
    /// </summary>
    /// <param name="id">The ID of the consumption record.</param>
    /// <returns>The <see cref="Consumption"/> record if found; otherwise, <c>null</c>.</returns>
    public override async Task<Consumption> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Meter)
            .Include(c => c.TariffBracket)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}
