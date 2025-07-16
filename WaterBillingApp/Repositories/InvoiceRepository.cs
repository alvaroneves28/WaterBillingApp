using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

/// <summary>
/// Repository for managing <see cref="Invoice"/> entities.
/// </summary>
public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

    /// <summary>
    /// Gets an invoice by its identifier, including related consumption, tariff bracket, meter, and customer details.
    /// </summary>
    /// <param name="id">The invoice ID.</param>
    /// <returns>The invoice if found; otherwise, <c>null</c>.</returns>
    public async Task<Invoice> GetInvoiceByIdAsync(int id)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.TariffBracket)
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
                    .ThenInclude(m => m.Customer)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    /// <summary>
    /// Gets all invoices, including related consumption, meter, and customer details.
    /// </summary>
    /// <returns>A list of all invoices.</returns>
    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
                .ThenInclude(m => m.Customer)
            .ToListAsync();
    }

    /// <summary>
    /// Checks whether an invoice with the specified ID exists.
    /// </summary>
    /// <param name="id">The invoice ID.</param>
    /// <returns><c>true</c> if the invoice exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> InvoiceExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(i => i.Id == id);
    }

    /// <summary>
    /// Gets an invoice by its associated consumption ID.
    /// </summary>
    /// <param name="consumptionId">The consumption ID.</param>
    /// <returns>The invoice if found; otherwise, <c>null</c>.</returns>
    public async Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
            .FirstOrDefaultAsync(i => i.ConsumptionId == consumptionId);
    }

    /// <summary>
    /// Gets the pending invoice for a specific customer, ordered by the most recent issue date.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>The pending invoice if any; otherwise, <c>null</c>.</returns>
    public async Task<Invoice?> GetPendingInvoiceForCustomerAsync(int customerId)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
            .Where(i => i.Status == InvoiceStatus.Pending &&
                        i.Consumption.Meter.CustomerId == customerId)
            .OrderByDescending(i => i.IssueDate)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Gets all invoices for a specific customer, ordered by the most recent issue date.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <returns>A list of invoices for the customer.</returns>
    public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
            .Where(i => i.Consumption.Meter.CustomerId == customerId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public override async Task<Invoice> GetByIdAsync(int id)
    {
        return await GetInvoiceByIdAsync(id);
    }

    /// <inheritdoc/>
    public override async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await GetAllInvoicesAsync();
    }

    /// <summary>
    /// Adds a new invoice asynchronously and saves changes to the database.
    /// </summary>
    /// <param name="invoice">The invoice to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddInvoiceAsync(Invoice invoice)
    {
        await _dbSet.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }
}
