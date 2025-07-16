using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;

namespace WaterBillingApp.Controllers
{
    /// <summary>
    /// Controller responsible for managing customers. Only accessible to users with the Admin role.
    /// Includes functionality to create, edit, delete, and list customers.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerController"/> class.
        /// </summary>
        /// <param name="customerRepository">The repository for accessing and managing customer data.</param>
        public CustomerController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Displays a list of all customers in the system.
        /// </summary>
        /// <returns>The view with a list of customers.</returns>
        public async Task<IActionResult> Index()
        {
            // Retrieve all customers from the repository
            var customers = await _customerRepository.GetAllAsync();

            // Return the view with the list of customers
            return View(customers);
        }

        /// <summary>
        /// Returns the form for creating a new customer.
        /// </summary>
        /// <returns>The customer creation view.</returns>
        public IActionResult Create()
        {
            // Display the form for creating a new customer
            return View();
        }

        /// <summary>
        /// Handles the submission of the customer creation form.
        /// </summary>
        /// <param name="model">The customer data to create.</param>
        /// <returns>Redirects to the index view on success, or returns the view with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(Customer model)
        {
            // Check if the received model is valid
            if (ModelState.IsValid)
            {
                // Add the new customer to the database
                await _customerRepository.AddAsync(model);

                // Redirect to the index (customer list) after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If model is invalid, re-display the form with validation errors
            return View(model);
        }


        /// <summary>
        /// Returns the form for editing an existing customer.
        /// </summary>
        /// <param name="id">The ID of the customer to edit.</param>
        /// <returns>The edit view with the customer's data, or a 404 if not found.</returns>
        public async Task<IActionResult> Edit(int id)
        {
            // Find the customer by ID
            var customer = await _customerRepository.GetByIdAsync(id);

            // If not found, return 404 Not Found
            if (customer == null) return NotFound();

            // Display the edit form populated with the customer's data
            return View(customer);
        }


        /// <summary>
        /// Handles the submission of the customer edit form.
        /// </summary>
        /// <param name="model">The updated customer data.</param>
        /// <returns>Redirects to the index view on success, or returns the view with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Customer model)
        {
            // Validate the model
            if (ModelState.IsValid)
            {
                // Update the customer data in the database
                await _customerRepository.UpdateAsync(model);

                // Redirect to the customer list after successful update
                return RedirectToAction(nameof(Index));
            }

            // If model is invalid, return the form again with validation errors
            return View(model);
        }


        /// <summary>
        /// Displays a confirmation view before deleting a customer.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>The confirmation view, or a 404 if the customer is not found.</returns>
        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve the customer to be deleted
            var customer = await _customerRepository.GetByIdAsync(id);

            // If not found, return 404 Not Found
            if (customer == null) return NotFound();

            // Display a confirmation view before deleting the customer
            return View(customer);
        }


        /// <summary>
        /// Permanently deletes a customer after confirmation.
        /// Displays error messages if the customer is not found or if deletion is not possible due to related data.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>Redirects to the index view after deletion attempt.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Attempt to delete the customer from the database
                await _customerRepository.DeleteAsync(id);

                // Show success message after deletion
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            catch (KeyNotFoundException)
            {
                // Show error if the customer does not exist
                TempData["ErrorMessage"] = "Customer not found.";
            }
            catch (DbUpdateException)
            {
                // Show error if the customer cannot be deleted due to related data (e.g. meters or invoices)
                TempData["ErrorMessage"] = "Unable to delete the customer because there are associated records (meters, invoices or consumptions).";
            }

            // Redirect to the customer list
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Displays a welcome message for the customer area.
        /// (Currently unused in the main admin flow.)
        /// </summary>
        /// <returns>The welcome view with a predefined message.</returns>
        [HttpGet]
        public IActionResult Welcome()
        {
            // Set a welcome message to display in the view
            ViewBag.Message = "Welcome to your customer area!";

            // Show the welcome view
            return View();
        }


    }
}
