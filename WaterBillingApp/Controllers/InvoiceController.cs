using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

namespace WaterBillingApp.Controllers
{
    
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConsumptionRepository _consumptionRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository, IConsumptionRepository consumptionRepository)
        {
            _invoiceRepository = invoiceRepository;
            _consumptionRepository = consumptionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> IssueInvoice(int consumptionId)
        {
            var consumption = await _consumptionRepository.GetByIdAsync(consumptionId);
            if (consumption == null)
            {
                return NotFound("Consumption not found.");
            }

            
            var existingInvoice = await _invoiceRepository.GetInvoiceByConsumptionIdAsync(consumptionId);
            if (existingInvoice != null)
            {
                TempData["StatusMessage"] = "Invoice already issued for this consumption.";
                return RedirectToAction("InvoiceCreated"); 
            }

            var invoice = new Invoice
            {
                ConsumptionId = consumption.Id,
                IssueDate = DateTime.Now,
                TotalAmount = CalculateAmount(consumption),
                Status = InvoiceStatus.Pending
            };

            await _invoiceRepository.AddInvoiceAsync(invoice);

            TempData["StatusMessage"] = "Invoice issued successfully.";
            return RedirectToAction("Index","Invoice", new { createdInvoiceId = invoice.Id });
        }

        private decimal CalculateAmount(Consumption consumption)
        {
            if (consumption.TariffBracket == null)
            {
                throw new InvalidOperationException("TariffBracket is not set for this consumption.");
            }

            
            return consumption.Volume * consumption.TariffBracket.PricePerCubicMeter;
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }


    }
}
