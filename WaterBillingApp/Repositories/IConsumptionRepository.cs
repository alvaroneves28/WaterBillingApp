using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Defines repository methods specifically for managing <see cref="Consumption"/> entities.
    /// Extends the generic repository interface with consumption-specific operations.
    /// </summary>
    public interface IConsumptionRepository : IGenericRepository<Consumption>
    {
        /// <summary>
        /// Retrieves all consumption records associated with a given meter.
        /// </summary>
        /// <param name="meterId">The identifier of the meter.</param>
        /// <returns>A collection of <see cref="Consumption"/> records for the specified meter.</returns>
        Task<IEnumerable<Consumption>> GetByMeterIdAsync(int meterId);

        /// <summary>
        /// Determines whether any consumption records exist for a specific meter.
        /// </summary>
        /// <param name="meterId">The identifier of the meter.</param>
        /// <returns><c>true</c> if any consumption records exist; otherwise, <c>false</c>.</returns>
        Task<bool> AnyByMeterIdAsync(int meterId);
    }
}
