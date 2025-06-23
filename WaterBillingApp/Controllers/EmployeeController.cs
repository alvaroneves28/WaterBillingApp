using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

public class EmployeeController : Controller
{
    private readonly IMeterRepository _meterRepository;

    public EmployeeController(IMeterRepository meterRepository)
    {
        _meterRepository = meterRepository;
    }

    public async Task<IActionResult> Index()
    {
        var pendingMeters = await _meterRepository.GetPendingMetersAsync();
        var activeMeters = await _meterRepository.GetActiveMetersAsync();

        var viewModel = new MetersDashboardViewModel
        {
            PendingMeters = pendingMeters.Select(m => new MeterViewModel
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                InstallationDate = m.InstallationDate,
                CustomerName = m.Customer?.FullName ?? "N/A",
                IsActive = m.IsActive,
                CustomerId = m.CustomerId
            }),
            ActiveMeters = activeMeters.Select(m => new MeterViewModel
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                InstallationDate = m.InstallationDate,
                IsActive = m.IsActive,
                CustomerName = m.Customer?.FullName ?? "N/A",
                CustomerId = m.CustomerId,
                Status = m.Status,

                LastConsumption = m.Consumptions
               .OrderByDescending(c => c.Date)
               .Select(c => new ConsumptionViewModel
               {
                   Id = c.Id,
                   Reading = c.Reading,
                   Volume = c.Volume,
                   Date = c.Date
               })
               .FirstOrDefault(),

                LastConsumptionValue = m.Consumptions.OrderByDescending(c => c.Date).FirstOrDefault()?.Volume,
                LastConsumptionDate = m.Consumptions.OrderByDescending(c => c.Date).FirstOrDefault()?.Date
            })
        };

        return View(viewModel);
    }

    
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var meter = await _meterRepository.GetByIdAsync(id);
        if (meter == null)
            return NotFound();

        meter.IsActive = true;
        meter.Status = MeterStatus.Approved;
        await _meterRepository.UpdateAsync(meter);

        TempData["StatusMessage"] = "Meter request approved successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var meter = await _meterRepository.GetByIdAsync(id);
        if (meter == null)
            return NotFound();

        await _meterRepository.DeleteAsync(id);

        TempData["StatusMessage"] = "Meter request was rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var meter = await _meterRepository.GetByIdAsync(id);
        if (meter == null)
        {
            return NotFound();
        }

        await _meterRepository.DeleteAsync(id);
        TempData["StatusMessage"] = "Meter deleted successfully.";
        return RedirectToAction("ManageMeters");
    }




}
