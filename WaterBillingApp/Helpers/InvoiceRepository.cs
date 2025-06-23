using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice> GetInvoiceByIdAsync(int id)
    {
        return await _context.Invoices
            .Include(i => i.Consumption)
                .ThenInclude(c => c.TariffBracket)  
            .Include(i => i.Consumption)
                .ThenInclude(c => c.Meter)          
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
    {
        return await _context.Invoices
            .Include(i => i.Consumption)
            .ThenInclude(c => c.Meter)
            .ToListAsync();
    }

    public async Task AddInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> InvoiceExistsAsync(int id)
    {
        return await _context.Invoices.AnyAsync(i => i.Id == id);
    }

    public async Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId)
    {
        return await _context.Invoices
            .Include(i => i.Consumption)
            .ThenInclude(c => c.Meter)
            .FirstOrDefaultAsync(i => i.ConsumptionId == consumptionId);
    }

}
