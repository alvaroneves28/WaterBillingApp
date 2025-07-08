using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Controllers
{
    [Authorize(Roles = "Employee")]
    public class MeterController : Controller
    {
        private readonly MeterRepository _meterRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ConsumptionRepository _consumptionRepository;

        public MeterController(MeterRepository meterRepository, CustomerRepository customerRepository, ConsumptionRepository consumptionRepository)
        {
            _meterRepository = meterRepository;
            _customerRepository = customerRepository;
            _consumptionRepository = consumptionRepository;
        }

        // GET: Meter
        public async Task<IActionResult> Index()
        {
            var meters = await _meterRepository.GetAllAsync();
            return View(meters);
        }

        // GET: Meter/Create
        public async Task<IActionResult> Create()
        {
            var customers = await _customerRepository.GetAllAsync();

            var model = new MeterViewModel
            {

                InstallationDate = DateTime.Today, 
                CustomersList = customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                })
            };

            return View(model);
        }

        // POST: Meter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var meter = new Meter
                {
                    SerialNumber = model.SerialNumber,
                    InstallationDate = model.InstallationDate,
                    IsActive = model.IsActive,
                    CustomerId = model.CustomerId
                };

                await _meterRepository.AddAsync(meter);
                return RedirectToAction(nameof(Index));
            }

            
            var customers = await _customerRepository.GetAllAsync();
            model.CustomersList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            });
            return View(model);
        }



        // GET: Meter/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var meter = await _meterRepository.GetByIdAsync(id);
            if (meter == null) return NotFound();

            var customers = await _customerRepository.GetAllAsync();

            var model = new MeterViewModel
            {
                Id = meter.Id,
                SerialNumber = meter.SerialNumber,
                InstallationDate = meter.InstallationDate,
                IsActive = meter.IsActive,
                CustomerId = meter.CustomerId,
                CustomersList = customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                })
            };

            return View(model);
        }

        // POST: Meter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var meter = new Meter
                {
                    Id = model.Id,
                    SerialNumber = model.SerialNumber,
                    InstallationDate = model.InstallationDate,
                    IsActive = model.IsActive,
                    CustomerId = model.CustomerId
                };

                await _meterRepository.UpdateAsync(meter);
                return RedirectToAction(nameof(Index));
            }

            var customers = await _customerRepository.GetAllAsync();
            model.CustomersList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            });

            return View(model);
        }

        // POST: Meter/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meter = await _meterRepository.GetByIdAsync(id);
            if (meter == null)
            {
                return NotFound();
            }

            bool hasConsumptions = meter.Consumptions != null && meter.Consumptions.Any();
            
            if (hasConsumptions)
            {
                TempData["StatusMessage"] = "Cannot delete meter because it has consumption records.";
                return RedirectToAction(nameof(Index));
            }

            await _meterRepository.DeleteAsync(id);
            TempData["StatusMessage"] = "Meter deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
