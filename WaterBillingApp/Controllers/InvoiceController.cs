using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller responsible for managing invoices, including viewing, issuing, and generating signed PDFs.
    /// </summary>
    public class InvoiceController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvoiceController"/> class.
        /// </summary>
        /// <param name="invoiceRepository">Repository for invoice data access.</param>
        /// <param name="consumptionRepository">Repository for consumption data access.</param>
        /// <param name="notificationRepository">Repository for notifications.</param>
        /// <param name="customerRepository">Repository for customer data access.</param>
        public InvoiceController(IInvoiceRepository invoiceRepository, IConsumptionRepository consumptionRepository, INotificationRepository notificationRepository, ICustomerRepository customerRepository)
        {
            _invoiceRepository = invoiceRepository;
            _consumptionRepository = consumptionRepository;
            _notificationRepository = notificationRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Displays a list of all invoices.
        /// </summary>
        /// <returns>A view showing all invoices.</returns>
        public async Task<IActionResult> Index()
        {
            // Retrieve all invoices asynchronously
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();
            // Return the view displaying all invoices
            return View(invoices);
        }

        /// <summary>
        /// Issues a new invoice for a specific consumption record if one does not already exist.
        /// Also creates a notification for the customer.
        /// </summary>
        /// <param name="consumptionId">The ID of the consumption record to issue the invoice for.</param>
        /// <returns>Redirects to the Employee index with a status message.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IssueInvoice(int consumptionId)
        {
            // Check if an invoice already exists for this consumption
            var existingInvoice = await _invoiceRepository.GetInvoiceByConsumptionIdAsync(consumptionId);

            if (existingInvoice != null)
            {
                // Inform user that invoice is already issued and redirect
                TempData["StatusMessage"] = "Error: An invoice has already been issued for this consumption.";
                return RedirectToAction("Index", "Employee");
            }

            // Fetch the consumption record by ID
            var consumption = await _consumptionRepository.GetByIdAsync(consumptionId);
            if (consumption == null)
            {
                // Inform user if consumption record not found
                TempData["StatusMessage"] = "Error: Consumption record not found.";
                return RedirectToAction("Index", "Employee");
            }

            // Create a new invoice entity with calculated amount and status Pending
            var invoice = new Invoice
            {
                ConsumptionId = consumptionId,
                IssueDate = DateTime.Now,
                TotalAmount = CalculateAmount(consumption),
                Status = InvoiceStatus.Pending
            };

            // Add the new invoice to the repository
            await _invoiceRepository.AddInvoiceAsync(invoice);

            // Get the customer ID related to this consumption
            var clientId = consumption.Meter.CustomerId;
            // Create a notification for the customer about the new invoice
            var notification = new Notification
            {
                CustomerId = clientId,
                Message = $"New Invoice of {invoice.TotalAmount:C} issued in {invoice.IssueDate:dd/MM/yyyy}.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            // Save the notification
            await _notificationRepository.AddNotificationAsync(notification);
            await _notificationRepository.SaveChangesAsync();

            // Inform user of success and redirect
            TempData["StatusMessage"] = "Invoice successfully issued.";
            return RedirectToAction("Index", "Employee");
        }

        /// <summary>
        /// Calculates the total amount for the given consumption based on its tariff bracket.
        /// </summary>
        /// <param name="consumption">The consumption record to calculate the amount for.</param>
        /// <returns>The total amount as a decimal.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the tariff bracket is not set.</exception>
        private decimal CalculateAmount(Consumption consumption)
        {
            // Ensure tariff bracket is set before calculating amount
            if (consumption.TariffBracket == null)
            {
                throw new InvalidOperationException("TariffBracket is not set for this consumption.");
            }

            // Calculate total amount based on volume and price per cubic meter
            return consumption.Volume * consumption.TariffBracket.PricePerCubicMeter;
        }

        /// <summary>
        /// Displays all invoices to an employee user.
        /// </summary>
        /// <returns>The view listing invoices or 404 if none found.</returns>
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> InvoicesForEmployee()
        {
            // Get all invoices for employee view
            var invoices = await _invoiceRepository.GetAllInvoicesAsync();

            // Return 404 if no invoices found
            if (invoices == null || !invoices.Any())
                return NotFound();

            // Return the employee invoices view with invoices
            return View("InvoicesForEmployee", invoices);
        }

        /// <summary>
        /// Displays invoice details for the customer, ensuring they can only access their own invoices.
        /// </summary>
        /// <param name="id">Invoice ID.</param>
        /// <returns>The invoice details view or a forbidden/404 response.</returns>
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DetailsForCustomer(int id)
        {
            // Retrieve the invoice by ID
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return NotFound();

            // Get the logged-in user's ID
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Forbid();

            // Get the customer record for this user
            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer == null)
                return Forbid();

            // Check if the invoice belongs to the logged-in customer
            if (invoice.Consumption?.Meter?.CustomerId != customer.Id)
            {
                // Forbid access if not the owner
                return Forbid();
            }

            // Return the invoice details view for the customer
            return View("DetailsForCustomer", invoice);
        }

        /// <summary>
        /// Displays invoice details for employees.
        /// </summary>
        /// <param name="id">Invoice ID.</param>
        /// <returns>The invoice details view or redirects to error handler if not found.</returns>
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DetailsForEmployee(int id)
        {
            // Retrieve invoice by ID
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(id);

            // Redirect to 404 error page if not found
            if (invoice == null)
            {
                return RedirectToAction("HttpStatusCodeHandler", "Error", new { statusCode = 404 });
            }

            // Return the invoice details view for employee
            return View("DetailsForEmployee", invoice);
        }

        /// <summary>
        /// Generates and returns a signed PDF of an invoice.
        /// </summary>
        /// <param name="invoiceSignature">The invoice signature data including invoice ID and base64 signature.</param>
        /// <returns>A PDF file with the signed invoice or NotFound if invoice does not exist.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DownloadSignedPdf([FromBody] InvoiceSignature invoiceSignature)
        {
            // Retrieve the invoice using the provided invoice ID
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceSignature.InvoiceId);

            if (invoice == null)
                return NotFound();

            // Generate PDF bytes with embedded customer signature
            byte[] pdfBytes = GeneratePdfSigned(invoice, invoiceSignature.AssinaturaBase64);

            // Return the PDF file as a downloadable file with appropriate content type and filename
            return File(pdfBytes, "application/pdf", $"Fatura_{invoice.Id}.pdf");
        }

        /// <summary>
        /// Generates a PDF document with invoice details and the customer's signature image.
        /// </summary>
        /// <param name="invoice">The invoice to generate the PDF for.</param>
        /// <param name="assinaturaBase64">The customer's signature as a Base64-encoded image string.</param>
        /// <returns>The PDF file as a byte array.</returns>
        private byte[] GeneratePdfSigned(Invoice invoice, string assinaturaBase64)
        {
            using (PdfDocument document = new PdfDocument())
            {
                // Add a new page and get its graphics context
                PdfPage page = document.Pages.Add();
                PdfGraphics graphics = page.Graphics;

                // Define fonts for title and text
                PdfFont titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
                PdfFont textFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12);

                float y = 20;

                // Draw title and invoice details on the PDF
                graphics.DrawString("Water Invoice", titleFont, PdfBrushes.DarkBlue, new PointF(0, y));
                y += 30;

                graphics.DrawString($"Invoice ID: {invoice.Id}", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 20;
                graphics.DrawString($"Customer: {invoice.Consumption.Meter.Customer.FullName}", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 20;
                graphics.DrawString($"Issue Date: {invoice.IssueDate:dd/MM/yyyy}", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 20;
                graphics.DrawString($"Total Amount: {invoice.TotalAmount:C}", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 20;
                graphics.DrawString($"Status: {invoice.Status}", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 20;
                graphics.DrawString($"Consumption: {invoice.Consumption.Volume} m³", textFont, PdfBrushes.Black, new PointF(0, y));
                y += 40;

                if (!string.IsNullOrEmpty(assinaturaBase64))
                {
                    // Remove data URI prefix if present in the base64 string
                    if (assinaturaBase64.StartsWith("data:image"))
                    {
                        var base64Data = assinaturaBase64.Substring(assinaturaBase64.IndexOf(',') + 1);
                        assinaturaBase64 = base64Data;
                    }

                    // Convert base64 string to byte array and load it as a PDF image
                    byte[] signatureBytes = Convert.FromBase64String(assinaturaBase64);
                    using (MemoryStream imageStream = new MemoryStream(signatureBytes))
                    {
                        PdfBitmap signatureImage = new PdfBitmap(imageStream);

                        // Draw label and signature image on the PDF
                        graphics.DrawString("Customer Signature:", textFont, PdfBrushes.Black, new PointF(0, y));
                        y += 20;
                        graphics.DrawImage(signatureImage, new RectangleF(0, y, 200, 100));
                    }
                }

                // Save PDF to memory stream and return its byte array
                using (MemoryStream stream = new MemoryStream())
                {
                    document.Save(stream);
                    return stream.ToArray();
                }
            }
        }

    }
}
