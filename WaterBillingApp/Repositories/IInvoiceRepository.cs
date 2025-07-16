using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Interface for managing <see cref="Invoice"/> entities, extending generic repository functionality.
    /// </summary>
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        /// <summary>
        /// Gets an invoice by its identifier, including related details.
        /// </summary>
        /// <param name="id">The invoice ID.</param>
        /// <returns>The invoice if found; otherwise, <c>null</c>.</returns>
        Task<Invoice> GetInvoiceByIdAsync(int id);

        /// <summary>
        /// Retrieves all invoices including related entities.
        /// </summary>
        /// <returns>A collection of all invoices.</returns>
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();

        /// <summary>
        /// Checks if an invoice exists by its identifier.
        /// </summary>
        /// <param name="id">The invoice ID.</param>
        /// <returns><c>true</c> if the invoice exists; otherwise, <c>false</c>.</returns>
        Task<bool> InvoiceExistsAsync(int id);

        /// <summary>
        /// Gets an invoice associated with a specific consumption ID.
        /// </summary>
        /// <param name="consumptionId">The consumption ID.</param>
        /// <returns>The invoice if found; otherwise, <c>null</c>.</returns>
        Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId);

        /// <summary>
        /// Retrieves the pending invoice for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>The pending invoice if any; otherwise, <c>null</c>.</returns>
        Task<Invoice?> GetPendingInvoiceForCustomerAsync(int customerId);

        /// <summary>
        /// Retrieves all invoices for a given customer, ordered by issue date descending.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <returns>A collection of invoices.</returns>
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);

        /// <summary>
        /// Adds a new invoice asynchronously and persists changes.
        /// </summary>
        /// <param name="invoice">The invoice to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddInvoiceAsync(Invoice invoice);
    }
}
