using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WaterBilliangAppInvoiceAPI.Data;
using WaterBilliangAppInvoiceAPI.Data.Entities;
using WaterBillingApp.Data.Entities;

namespace WaterBilliangAppInvoiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InvoicesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Invoices/LastPending/5
        [HttpGet("last-invoice/{customerId}")]
        public async Task<ActionResult<LastInvoiceDTO>> GetLastInvoice(int customerId)
        {
            var lastInvoice = await _context.Invoices
                .Include(i => i.Consumption)
                    .ThenInclude(c => c.Meter)
                .Where(i => i.Consumption.Meter.CustomerId == customerId)
                .OrderByDescending(i => i.IssueDate)
                .FirstOrDefaultAsync();

            if (lastInvoice == null)
                return NotFound();

            var dto = new LastInvoiceDTO
            {
                Id = lastInvoice.Id,
                IssueDate = lastInvoice.IssueDate,
                Status = lastInvoice.Status,
                TotalAmount = lastInvoice.TotalAmount,
                ConsumptionValue = lastInvoice.Consumption != null ? lastInvoice.Consumption.Volume : 0
            };

            return Ok(dto);
        }



    }
}
