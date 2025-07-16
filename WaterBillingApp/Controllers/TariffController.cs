using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller responsible for managing tariff brackets,
    /// including listing, creating, editing, and deleting tariffs.
    /// </summary>
    public class TariffController : Controller
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TariffController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public TariffController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays all tariff brackets.
        /// </summary>
        /// <returns>A view with a list of tariff brackets.</returns>
        public async Task<IActionResult> ManageTariffs()
        {
            // Retrieves all tariff brackets from the database
            var tariffs = await _context.TariffBrackets.ToListAsync();
            return View(tariffs);
        }

        /// <summary>
        /// Shows the form to create a new tariff bracket.
        /// </summary>
        public IActionResult Create()
        {
            // Simply returns the empty form view
            return View();
        }

        /// <summary>
        /// Handles the submission of a new tariff bracket.
        /// </summary>
        /// <param name="model">The tariff bracket model submitted by the user.</param>
        [HttpPost]
        public async Task<IActionResult> Create(TariffBracket model)
        {
            if (ModelState.IsValid)
            {
                // Adds the new tariff to the database
                _context.TariffBrackets.Add(model);
                await _context.SaveChangesAsync();

                // Redirects back to the tariff list
                return RedirectToAction(nameof(ManageTariffs));
            }

            // If validation fails, redisplay the form with errors
            return View(model);
        }

        /// <summary>
        /// Shows the form to edit an existing tariff bracket.
        /// </summary>
        /// <param name="id">The ID of the tariff bracket to edit.</param>
        public async Task<IActionResult> Edit(int id)
        {
            // Finds the tariff by its ID
            var tariff = await _context.TariffBrackets.FindAsync(id);
            if (tariff == null) return NotFound();

            // Returns the edit form with existing data
            return View(tariff);
        }

        /// <summary>
        /// Handles the submission of edited tariff bracket data.
        /// </summary>
        /// <param name="model">The updated tariff bracket model.</param>
        [HttpPost]
        public async Task<IActionResult> Edit(TariffBracket model)
        {
            if (!ModelState.IsValid)
                return View(model); // Return the form if the model is invalid

            // Find the existing record in the database
            var existing = await _context.TariffBrackets.FindAsync(model.Id);
            if (existing == null) return NotFound();

            // Update the fields with new values
            existing.MinVolume = model.MinVolume;
            existing.MaxVolume = model.MaxVolume;
            existing.PricePerCubicMeter = model.PricePerCubicMeter;

            // Save the changes
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTariffs));
        }

        /// <summary>
        /// Deletes a tariff bracket by ID (GET version).
        /// </summary>
        /// <param name="id">The ID of the tariff bracket to delete.</param>
        public async Task<IActionResult> Delete(int id)
        {
            // Locate the tariff by ID
            var tariff = await _context.TariffBrackets.FindAsync(id);
            if (tariff == null) return NotFound();

            // Delete it from the database
            _context.TariffBrackets.Remove(tariff);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageTariffs));
        }

        /// <summary>
        /// Confirms and executes the deletion of a tariff bracket.
        /// Typically triggered by a form submission.
        /// </summary>
        /// <param name="id">The ID of the tariff bracket to delete.</param>
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tariff = await _context.TariffBrackets.FindAsync(id);
            if (tariff == null) return NotFound();

            // Delete and save
            _context.TariffBrackets.Remove(tariff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTariffs));
        }
    }
}
