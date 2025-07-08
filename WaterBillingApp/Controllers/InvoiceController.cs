using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Controllers
{

    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICustomerRepository _customerRepository;

        public InvoiceController(IInvoiceRepository invoiceRepository, IConsumptionRepository consumptionRepository, INotificationRepository notificationRepository, ICustomerRepository customerRepository)
        {
            _invoiceRepository = invoiceRepository;
            _consumptionRepository = consumptionRepository;
            _notificationRepository = notificationRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            return View(invoices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueInvoice(int consumptionId)
        {
            var existingInvoice = await _invoiceRepository.GetInvoiceByConsumptionIdAsync(consumptionId);

            if (existingInvoice != null)
            {
                TempData["StatusMessage"] = "Error: An invoice has already been issued for this consumption.";
                return RedirectToAction("Index", "Employee");
            }

            var consumption = await _consumptionRepository.GetByIdAsync(consumptionId);
            if (consumption == null)
            {
                TempData["StatusMessage"] = "Error: Consumption record not found.";
                return RedirectToAction("Index", "Employee");
            }

            var invoice = new Invoice
            {
                ConsumptionId = consumptionId,
                IssueDate = DateTime.Now,
                TotalAmount = CalculateAmount(consumption),
                Status = InvoiceStatus.Pending
            };

            await _invoiceRepository.AddInvoiceAsync(invoice);

            
            var clientId = consumption.Meter.CustomerId; 
            var notification = new Notification
            {
                CustomerId = clientId,
                Message = $"New Invoice of {invoice.TotalAmount:C} issued in {invoice.IssueDate:dd/MM/yyyy}.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await _notificationRepository.AddNotificationAsync(notification); 
            await _notificationRepository.SaveAsync(); 

            TempData["StatusMessage"] = "Invoice successfully issued.";
            return RedirectToAction("Index", "Employee");
        }


        private decimal CalculateAmount(Consumption consumption)
        {
            if (consumption.TariffBracket == null)
            {
                throw new InvalidOperationException("TariffBracket is not set for this consumption.");
            }


            return consumption.Volume * consumption.TariffBracket.PricePerCubicMeter;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> InvoicesForEmployee()
        {
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            if (invoices == null || !invoices.Any())
                return NotFound();

            return View("InvoicesForEmployee", invoices);
        }


        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DetailsForCustomer(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();

           
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            
            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer == null)
                return Forbid();

           
            if (invoice.Consumption?.Meter?.CustomerId != customer.Id)
            {
                return Forbid();
            }

            return View("DetailsForCustomer", invoice);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DetailsForEmployee(int id)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);

            if (invoice == null)
            {
                return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCode = 404 });
            }

            return View("DetailsForEmployee", invoice);
        }


    }
}
