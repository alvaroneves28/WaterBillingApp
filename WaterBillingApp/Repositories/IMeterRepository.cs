using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Defines repository methods specific to <see cref="Meter"/> entities.
    /// </summary>
    public interface IMeterRepository : IGenericRepository<Meter>
    {
        /// <summary>
        /// Retrieves all meters with a status of pending.
        /// </summary>
        /// <returns>A collection of pending meters.</returns>
        Task<IEnumerable<Meter>> GetPendingMetersAsync();

        /// <summary>
        /// Retrieves all active and approved meters.
        /// </summary>
        /// <returns>A collection of active meters.</returns>
        Task<IEnumerable<Meter>> GetActiveMetersAsync();

        /// <summary>
        /// Retrieves all active meters for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A collection of meters for the customer.</returns>
        Task<IEnumerable<Meter>> GetMetersByCustomerAsync(int customerId);

        /// <summary>
        /// Saves any pending changes to the database.
        /// </summary>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task SaveAsync();
    }
}
