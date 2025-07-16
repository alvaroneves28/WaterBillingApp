using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

/// <summary>
/// Controller responsible for managing consumption readings, including adding manual and automatic readings.
/// </summary>
public class ConsumptionController : Controller
{
    private readonly IConsumptionRepository _consumptionRepository;
    private readonly IMeterRepository _meterRepository;
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumptionController"/> class.
    /// </summary>
    /// <param name="consumptionRepository">The repository for managing consumption data.</param>
    /// <param name="meterRepository">The repository for accessing meter data.</param>
    /// <param name="context">The application's database context.</param>
    public ConsumptionController(IConsumptionRepository consumptionRepository, IMeterRepository meterRepository, ApplicationDbContext context)
    {
        _consumptionRepository = consumptionRepository;
        _meterRepository = meterRepository;
        _context = context;
    }

    /// <summary>
    /// Displays the form for adding a manual reading. If no reading exists for the current month after the 20th,
    /// an automatic reading is calculated and saved based on the last recorded volume.
    /// </summary>
    /// <param name="meterId">The ID of the meter to add a reading for.</param>
    /// <returns>The view for adding a manual reading, or redirects after automatic entry.</returns>
    [HttpGet]
    public async Task<IActionResult> AddReading(int meterId)
    {
        // Get the meter from the database by its ID
        var meter = await _meterRepository.GetByIdAsync(meterId);

        // Return 404 if the meter is not found or is inactive
        if (meter == null || !meter.IsActive)
            return NotFound();

        // Determine the start and end of the current month
        var today = DateTime.Today;
        var currentMonthStart = new DateTime(today.Year, today.Month, 1);
        var nextMonthStart = currentMonthStart.AddMonths(1);

        // Check if the meter already has a reading for the current month
        var hasCurrentMonthReading = meter.Consumptions
            .Any(c => c.Date >= currentMonthStart && c.Date < nextMonthStart);

        // If it's after the 20th of the month and there's no reading, generate an automatic one
        if (today.Day > 20 && !hasCurrentMonthReading)
        {
            // Get the most recent consumption record
            var lastConsumption = meter.Consumptions
                .OrderByDescending(c => c.Date)
                .FirstOrDefault();

            // Use 0 if no previous reading exists
            int lastReading = lastConsumption?.Reading ?? 0;
            int lastVolume = (int)(lastConsumption?.Volume ?? 0);

            // Estimate the new reading by adding the last volume
            var estimatedReading = lastReading + lastVolume;

            // Find the tariff bracket that matches the volume
            var tariffBracket = await _context.TariffBrackets
                .Where(tb => tb.MinVolume <= lastVolume &&
                            (tb.MaxVolume == null || lastVolume <= tb.MaxVolume))
                .OrderBy(tb => tb.MinVolume)
                .FirstOrDefaultAsync();

            // Create a new automatic consumption entry
            var autoConsumption = new Consumption
            {
                MeterId = meter.Id,
                Date = today,
                Reading = estimatedReading,
                Volume = lastVolume,
                TariffBracket = tariffBracket
            };

            // Save the new automatic consumption
            await _consumptionRepository.AddAsync(autoConsumption);

            // Show a message to the user and redirect
            TempData["StatusMessage"] = "No manual reading found. Automatic consumption was registered.";
            return RedirectToAction("Index", "CustomerArea");
        }

        // If manual entry is needed, create the model and show the form
        var model = new AddReadingViewModel
        {
            MeterId = meter.Id,
            SerialNumber = meter.SerialNumber,
            Date = DateTime.Today
        };

        return View(model);
    }

    /// <summary>
    /// Submits a new manual consumption reading for the specified meter. Validates the reading against the previous one
    /// and assigns the appropriate tariff bracket based on the volume used.
    /// </summary>
    /// <param name="model">The consumption data submitted from the form.</param>
    /// <returns>Redirects to the customer area if successful; otherwise, redisplays the form with validation messages.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReading(AddConsumptionViewModel model)
    {
        // Validate model state
        if (!ModelState.IsValid)
            return View(model);

        // Get the meter from the database
        var meter = await _meterRepository.GetByIdAsync(model.MeterId);

        // Return 404 if the meter is not found or inactive
        if (meter == null || !meter.IsActive)
            return NotFound();

        // Get the most recent consumption for this meter
        var lastConsumption = meter.Consumptions
            .OrderByDescending(c => c.Date)
            .FirstOrDefault();

        // Get the last reading, default to 0 if no previous consumption
        var lastReading = lastConsumption?.Reading ?? 0;

        // Ensure the new reading is greater than the last
        if (model.Reading <= lastReading)
        {
            ModelState.AddModelError("Reading", $"The new reading must be greater than the last recorded reading ({lastReading}).");
            return View(model);
        }

        // Calculate volume consumed
        var volume = model.Reading - lastReading;

        // Find the appropriate tariff bracket based on the volume
        var tariffBracket = await _context.TariffBrackets
            .Where(tb => tb.MinVolume <= volume &&
                        (tb.MaxVolume == null || volume <= tb.MaxVolume))
            .OrderBy(tb => tb.MinVolume)
            .FirstOrDefaultAsync();

        // If no matching tariff bracket is found, show an error
        if (tariffBracket == null)
        {
            ModelState.AddModelError(string.Empty, "No applicable tariff bracket found for this volume.");
            return View(model);
        }

        // Create the new consumption record
        var consumption = new Consumption
        {
            MeterId = model.MeterId,
            Date = model.Date,
            Reading = model.Reading,
            Volume = volume,
            TariffBracket = tariffBracket
        };

        // Save the new consumption entry
        await _consumptionRepository.AddAsync(consumption);

        // Inform the user and redirect
        TempData["StatusMessage"] = "Reading added successfully!";
        return RedirectToAction("Index", "CustomerArea");
    }
}
