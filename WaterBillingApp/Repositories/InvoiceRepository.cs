using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

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

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
                .ThenInclude(m => m.Customer)
            .ToListAsync();
    }

    public async Task<bool> InvoiceExistsAsync(int id)
    {
        return await _dbSet.AnyAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
            .FirstOrDefaultAsync(i => i.ConsumptionId == consumptionId);
    }

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

    public async Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
    {
        return await _dbSet
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)
            .Where(i => i.Consumption.Meter.CustomerId == customerId)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync();
    }

  
    public override async Task<Invoice> GetByIdAsync(int id)
    {
        return await GetInvoiceByIdAsync(id);
    }

   
    public override async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await GetAllInvoicesAsync();
    }

    public async Task AddInvoiceAsync(Invoice invoice)
    {
        await _dbSet.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }
}
