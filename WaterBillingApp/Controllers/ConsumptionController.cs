using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

public class ConsumptionController : Controller
{
    private readonly IConsumptionRepository _consumptionRepository;
    private readonly IMeterRepository _meterRepository;

    public ConsumptionController(IConsumptionRepository consumptionRepository, IMeterRepository meterRepository)
    {
        _consumptionRepository = consumptionRepository;
        _meterRepository = meterRepository;
    }

    [HttpGet]
    public async Task<IActionResult> AddReading(int meterId)
    {
        var meter = await _meterRepository.GetByIdAsync(meterId);
        if (meter == null || !meter.IsActive)
            return NotFound();

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
    public async Task<IActionResult> AddReading(AddReadingViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var meter = await _meterRepository.GetByIdAsync(model.MeterId);
        if (meter == null || !meter.IsActive)
            return NotFound();

        var consumption = new Consumption
        {
            MeterId = model.MeterId,
            Volume = model.Volume,
            Date = model.Date
        };

        await _consumptionRepository.AddAsync(consumption);

        TempData["StatusMessage"] = "Reading added successfully!";
        return RedirectToAction("AddReading", "Consumption");
    }
}
