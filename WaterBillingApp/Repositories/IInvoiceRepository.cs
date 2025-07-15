using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<Invoice> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<bool> InvoiceExistsAsync(int id);
        Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId);
        Task<Invoice?> GetPendingInvoiceForCustomerAsync(int customerId);
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);

        Task AddInvoiceAsync(Invoice invoice);

    }
}
