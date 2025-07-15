using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

public class ConsumptionController : Controller
{
    private readonly IConsumptionRepository _consumptionRepository;
    private readonly IMeterRepository _meterRepository;
    private readonly ApplicationDbContext _context;

    public ConsumptionController(IConsumptionRepository consumptionRepository, IMeterRepository meterRepository, ApplicationDbContext context)
    {
        _consumptionRepository = consumptionRepository;
        _meterRepository = meterRepository;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AddReading(int meterId)
    {
        var meter = await _meterRepository.GetByIdAsync(meterId);
        if (meter == null || !meter.IsActive)
            return NotFound();

        var today = DateTime.Today;
        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
        var nextMonthStart = currentMonthStart.AddMonths(1);

        var hasCurrentMonthReading = meter.Consumptions
            .Any(c => c.Date >= currentMonthStart && c.Date < nextMonthStart);

      
        if (today.Day > 20 && !hasCurrentMonthReading)
        {
            var lastConsumption = meter.Consumptions
                .OrderByDescending(c => c.Date)
                .FirstOrDefault();

            int lastReading = lastConsumption?.Reading ?? 0;
            int lastVolume = (int)(lastConsumption?.Volume ?? 0);

           
            var estimatedReading = lastReading + lastVolume;

            var tariffBracket = await _context.TariffBrackets
                .Where(tb => tb.MinVolume <= lastVolume &&
                            (tb.MaxVolume == null || lastVolume <= tb.MaxVolume))
                .OrderBy(tb => tb.MinVolume)
                .FirstOrDefaultAsync();

            var autoConsumption = new Consumption
            {
                MeterId = meter.Id,
                Date = today,
                Reading = estimatedReading,
                Volume = lastVolume,
                TariffBracket = tariffBracket
            };

            await _consumptionRepository.AddAsync(autoConsumption);

            TempData["StatusMessage"] = "No manual reading found. Automatic consumption was registered.";
            return RedirectToAction("Index", "CustomerArea");
        }

        var model = new AddReadingViewModel
        {
            MeterId = meter.Id,
            SerialNumber = meter.SerialNumber,
            Date = DateTime.Today
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReading(AddConsumptionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var meter = await _meterRepository.GetByIdAsync(model.MeterId);
        if (meter == null || !meter.IsActive)
            return NotFound();

        var lastConsumption = meter.Consumptions
            .OrderByDescending(c => c.Date)
            .FirstOrDefault();

        var lastReading = lastConsumption?.Reading ?? 0;

        if (model.Reading <= lastReading)
        {
            ModelState.AddModelError("Reading", $"The new reading must be greater than the last recorded reading ({lastReading}).");
            return View(model);
        }

        var volume = model.Reading - lastReading;

       
        var tariffBracket = await _context.TariffBrackets
            .Where(tb => tb.MinVolume <= volume &&
                        (tb.MaxVolume == null || volume <= tb.MaxVolume))
            .OrderBy(tb => tb.MinVolume) 
            .FirstOrDefaultAsync();

        if (tariffBracket == null)
        {
            ModelState.AddModelError(string.Empty, "No applicable tariff bracket found for this volume.");
            return View(model);
        }

        var consumption = new Consumption
        {
            MeterId = model.MeterId,
            Date = model.Date,
            Reading = model.Reading,
            Volume = volume,
            TariffBracket = tariffBracket
        };

        await _consumptionRepository.AddAsync(consumption);

        TempData["StatusMessage"] = "Reading added successfully!";
        return RedirectToAction("Index", "CustomerArea");
    }
}
