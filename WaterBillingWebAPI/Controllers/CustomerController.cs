using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WaterBillingApp.Data.Entities;
using WaterBillingWebAPI.Data;
using WaterBillingWebAPI.Model.DTO;

namespace WaterBillingWebAPI.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        [HttpPost("consumptions")]
        public async Task<IActionResult> SubmitConsumption([FromBody] CreateConsumptionDTO dto)
        {
            var deadline = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 25); 
            if (DateTime.Now > deadline)
            {
                return BadRequest(new { message = "The submission period for this month has ended." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var meter = await _context.Meters.FirstOrDefaultAsync(m => m.Id == dto.MeterId && m.Customer.ApplicationUserId == userId);

            if (meter == null)
                return NotFound(new { message = "Meter not found or does not belong to this client." });

            var consumption = new Consumption
            {
                MeterId = dto.MeterId,
                Date = dto.Date,
                Volume = dto.Value
            };

            _context.Consumptions.Add(consumption);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Consumption registered successfully." });
        }


        [HttpGet("invoices")]
        public async Task<IActionResult> GetClientInvoices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null)
                return Unauthorized();

            var invoices = await _context.Invoices
                .Include(i => i.Consumption)
                    .ThenInclude(c => c.Meter)
                .Where(i => i.Consumption.Meter.CustomerId == client.Id)
                .Select(i => new InvoiceDTO
                {
                    Id = i.Id,
                    IssueDate = i.IssueDate,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status
                })
                .ToListAsync();

            return Ok(invoices);
        }


        [Authorize]
        [HttpGet("consumptions/history")]
        public async Task<IActionResult> GetConsumptionHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = await _context.Customers
                .Include(c => c.Meters)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null)
                return Unauthorized();

            var meterIds = client.Meters.Select(m => m.Id).ToList();

            var history = await _context.Consumptions
                .Where(c => meterIds.Contains(c.MeterId))
                .OrderByDescending(c => c.Date)
                .Select(c => new ConsumptionHistoryDTO
                {
                    MeterId = c.MeterId,
                    Date = c.Date,
                    Volume = c.Volume
                })
                .ToListAsync();

            return Ok(history);
        }

        [HttpGet("invoices/unread")]
        public async Task<IActionResult> GetUnreadInvoices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);
            if (client == null)
                return Unauthorized();

            var invoices = await _context.Invoices
                .Include(i => i.Consumption)
                    .ThenInclude(c => c.Meter)
                .Where(i => i.Consumption.Meter.CustomerId == client.Id && !i.IsRead)
                .Select(i => new InvoiceDTO
                {
                    Id = i.Id,
                    IssueDate = i.IssueDate,
                    TotalAmount = i.TotalAmount,
                    Status = i.Status
                })
                .ToListAsync();

            return Ok(invoices);
        }

    
        [HttpPost("invoices/{id}/mark-as-read")]
        public async Task<IActionResult> MarkInvoiceAsRead(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = await _context.Customers.FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            var invoice = await _context.Invoices
                .Include(i => i.Consumption)
                    .ThenInclude(c => c.Meter)
                .FirstOrDefaultAsync(i => i.Id == id && i.Consumption.Meter.CustomerId == client.Id);

            if (invoice == null)
                return NotFound();

            invoice.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Invoice marked as read." });
        }

        [AllowAnonymous]
        [HttpPost("meter-requests/anonymous")]
        public async Task<IActionResult> SubmitAnonymousMeterRequest([FromBody] AnonymousMeterRequestDTO requestDto)
        {
            // Verifica se já existe pedido ou cliente com o mesmo email ou NIF
            var exists = await _context.Customers.AnyAsync(c => c.Email == requestDto.Email || c.NIF == requestDto.NIF);
            if (exists)
                return Conflict("Já existe um cliente com este email ou NIF.");

            // Cria um novo pedido de contador
            var meterRequest = new MeterRequest
            {
                RequesterName = requestDto.Name,
                Address = requestDto.Address,
                NIF = requestDto.NIF,
                Phone = requestDto.PhoneNumber,
                RequestDate = DateTime.UtcNow,
                Status = MeterRequestStatus.Pending,
            };

            _context.MeterRequests.Add(meterRequest);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pedido submetido com sucesso. Irá ser analisado por um funcionário." });
        }

        [Authorize]
        [HttpGet("meters/status")]
        public async Task<IActionResult> GetMeterStatus()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var client = await _context.Customers
                .Include(c => c.Meters)
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (client == null)
                return Unauthorized();

            var meters = client.Meters
                .Select(m => new MeterStatusDTO
                {
                    Id = m.Id,
                    InstallationAddress = m.Customer.Address,
                    RequestDate = m.InstallationDate, 
                    Status = m.Status
                })
                .ToList();

            return Ok(meters);
        }


        [AllowAnonymous]
        [HttpGet("tariff-brackets")]
        public async Task<IActionResult> GetTariffBrackets()
        {
            var brackets = await _context.TariffBrackets
                .OrderBy(tb => tb.MinVolume)
                .Select(tb => new TariffDTO
                {
                    MinVolume = tb.MinVolume,
                    MaxVolume = tb.MaxVolume,
                    PricePerCubicMeter = tb.PricePerCubicMeter
                })
                .ToListAsync();

            return Ok(brackets);
        }

        [AllowAnonymous]
        [HttpPost("meters/status-anonymous")]
        public async Task<IActionResult> GetAnonymousMeterStatus([FromBody] AnonymousMeterStatusRequestDTO dto)
        {
            var meters = await _context.Meters
                .Where(m => m.Customer.Email == dto.Email && m.Customer.NIF == dto.NIF)
                .Select(m => new AnonymousMeterStatusDTO
                {
                    MeterId = m.Id,
                    Address = m.Customer.Address,
                    RequestDate = m.InstallationDate,
                    Status = m.Status
                })
                .ToListAsync();

            if (!meters.Any())
            {
                return NotFound(new { message = "Não foram encontrados pedidos com esses dados." });
            }

            return Ok(meters);
        }

        [HttpGet("mine")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MeterDTO>>> GetMyMeters()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.ApplicationUserId == userId);

            if (customer == null)
                return Unauthorized();

            var meters = await _context.Meters
                .Where(m => m.CustomerId == customer.Id)
                .Select(m => new MeterDTO
                {
                    Id = m.Id
                })
                .ToListAsync();

            return Ok(meters);
        }


    }
}
