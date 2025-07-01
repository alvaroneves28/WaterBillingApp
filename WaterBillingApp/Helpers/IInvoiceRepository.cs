using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface IInvoiceRepository
    {
        Task<Invoice> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task AddInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(int id);
        Task<bool> InvoiceExistsAsync(int id);
        Task<Invoice?> GetInvoiceByConsumptionIdAsync(int consumptionId);
        Task<Invoice?> GetPendingInvoiceForCustomerAsync(int customerId);
        Task<IEnumerable<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);

    }
}
