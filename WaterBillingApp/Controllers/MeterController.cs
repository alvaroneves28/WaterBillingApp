using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Models;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller for managing meters.
    /// Allows employees to create, edit, list, and delete meters.
    /// </summary>
    [Authorize(Roles = "Employee")]
    public class MeterController : Controller
    {
        private readonly MeterRepository _meterRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly ConsumptionRepository _consumptionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MeterController"/> class.
        /// </summary>
        /// <param name="meterRepository">Repository for meter data access.</param>
        /// <param name="customerRepository">Repository for customer data access.</param>
        /// <param name="consumptionRepository">Repository for consumption data access.</param>
        public MeterController(MeterRepository meterRepository, CustomerRepository customerRepository, ConsumptionRepository consumptionRepository)
        {
            _meterRepository = meterRepository;
            _customerRepository = customerRepository;
            _consumptionRepository = consumptionRepository;
        }

        /// <summary>
        /// Displays a list of all meters.
        /// </summary>
        /// <returns>View with the list of meters.</returns>
        // GET: Meter
        public async Task<IActionResult> Index()
        {
            var meters = await _meterRepository.GetAllAsync(); // Fetch all meters from the database.
            return View(meters); // Return view with the list of meters.
        }

        /// <summary>
        /// Displays the form for creating a new meter.
        /// </summary>
        /// <returns>The create meter view with customers dropdown populated.</returns>
        // GET: Meter/Create
        public async Task<IActionResult> Create()
        {
            var customers = await _customerRepository.GetAllAsync(); // Fetch all customers to populate dropdown.

            var model = new MeterViewModel
            {
                InstallationDate = DateTime.Today, // Default the installation date to today.
                CustomersList = customers.Select(c => new SelectListItem // Create dropdown list items for customers.
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                })
            };

            return View(model); // Return the view with the view model.
        }


        /// <summary>
        /// Handles POST request to create a new meter.
        /// </summary>
        /// <param name="model">The meter data submitted by the user.</param>
        /// <returns>Redirects to the Index action on success or redisplays the form with errors.</returns>
        // POST: Meter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a new Meter entity from the view model.
                var meter = new Meter
                {
                    SerialNumber = model.SerialNumber,
                    InstallationDate = model.InstallationDate,
                    IsActive = model.IsActive,
                    CustomerId = model.CustomerId
                };

                await _meterRepository.AddAsync(meter); // Save the meter to the database.
                return RedirectToAction(nameof(Index)); // Redirect to the meter list.
            }

            // If model is invalid, repopulate customer dropdown and return view.
            var customers = await _customerRepository.GetAllAsync();
            model.CustomersList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            });
            return View(model);
        }

        /// <summary>
        /// Displays the form to edit an existing meter.
        /// </summary>
        /// <param name="id">The ID of the meter to edit.</param>
        /// <returns>The edit view with meter data prefilled or NotFound if the meter does not exist.</returns>
        // GET: Meter/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var meter = await _meterRepository.GetByIdAsync(id); // Get the meter by ID.
            if (meter == null) return NotFound(); // Return 404 if not found.

            var customers = await _customerRepository.GetAllAsync(); // Get customers for dropdown.

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

            return View(model); // Return view with meter data prefilled.
        }

        /// <summary>
        /// Handles POST request to update an existing meter.
        /// </summary>
        /// <param name="model">The updated meter data submitted by the user.</param>
        /// <returns>Redirects to Index on success or redisplays the edit form on error.</returns>
        // POST: Meter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create a meter object from the updated data.
                var meter = new Meter
                {
                    Id = model.Id,
                    SerialNumber = model.SerialNumber,
                    InstallationDate = model.InstallationDate,
                    IsActive = model.IsActive,
                    CustomerId = model.CustomerId
                };

                await _meterRepository.UpdateAsync(meter); // Update the meter in the database.
                return RedirectToAction(nameof(Index)); // Redirect to meter list.
            }

            // If model is invalid, repopulate dropdown and return view.
            var customers = await _customerRepository.GetAllAsync();
            model.CustomersList = customers.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            });

            return View(model);
        }

        /// <summary>
        /// Handles POST request to delete a meter by ID.
        /// Prevents deletion if the meter has associated consumption records.
        /// </summary>
        /// <param name="id">The ID of the meter to delete.</param>
        /// <returns>Redirects to Index with status message.</returns>
        // POST: Meter/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meter = await _meterRepository.GetByIdAsync(id); // Get the meter by ID.
            if (meter == null)
            {
                return NotFound(); // Return 404 if not found.
            }

            // Check if the meter has any consumption records.
            bool hasConsumptions = meter.Consumptions != null && meter.Consumptions.Any();

            if (hasConsumptions)
            {
                TempData["StatusMessage"] = "Cannot delete meter because it has consumption records.";
                return RedirectToAction(nameof(Index)); // Prevent deletion if data exists.
            }

            await _meterRepository.DeleteAsync(id); // Delete the meter.
            TempData["StatusMessage"] = "Meter deleted successfully.";
            return RedirectToAction(nameof(Index)); // Redirect to the list with a success message.
        }
    }
}
