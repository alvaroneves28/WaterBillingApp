using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Helpers;
using WaterBillingApp.Models;

/// <summary>
/// Controller responsible for managing meter requests, approvals, rejections,
/// and displaying meter and notification dashboards. Intended for employee use.
/// </summary>
public class EmployeeController : Controller
{
    private readonly IMeterRepository _meterRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMeterRequestRepository _meterRequestRepository;
    private readonly ICustomerRepository _customerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeController"/> class.
    /// </summary>
    /// <param name="meterRepository">Repository for meter data access.</param>
    /// <param name="notificationRepository">Repository for notification data access.</param>
    /// <param name="meterRequestRepository">Repository for external meter requests.</param>
    /// <param name="customerRepository">Repository for customer data access.</param>
    public EmployeeController(IMeterRepository meterRepository, INotificationRepository notificationRepository, IMeterRequestRepository meterRequestRepository, ICustomerRepository customerRepository)
    {
        _meterRepository = meterRepository;
        _notificationRepository = notificationRepository;
        _meterRequestRepository = meterRequestRepository;
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// Displays the employee dashboard with pending and active meters,
    /// and pending meter requests.
    /// </summary>
    /// <returns>The dashboard view populated with meter and request information.</returns>
    public async Task<IActionResult> Index()
    {
        // Get all meters that are pending approval
        var pendingMeters = await _meterRepository.GetPendingMetersAsync();

        // Get all meters that are currently active
        var activeMeters = await _meterRepository.GetActiveMetersAsync();

        // Get all pending meter requests submitted externally
        var pendingMeterRequests = await _meterRequestRepository.GetPendingRequestsAsync();

        // Build the view model to pass to the view
        var viewModel = new MetersDashboardViewModel
        {
            // Project pending meters into the view model
            PendingMeters = pendingMeters.Select(m => new MeterViewModel
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                InstallationDate = m.InstallationDate,
                CustomerName = m.Customer?.FullName ?? "N/A",
                IsActive = m.IsActive,
                CustomerId = m.CustomerId
            }),

            // Project active meters into the view model, including last consumption info
            ActiveMeters = activeMeters.Select(m => new MeterViewModel
            {
                Id = m.Id,
                SerialNumber = m.SerialNumber,
                InstallationDate = m.InstallationDate,
                IsActive = m.IsActive,
                CustomerName = m.Customer?.FullName ?? "N/A",
                CustomerId = m.CustomerId,
                Status = m.Status,

                // Get the most recent consumption record
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

                // Get last consumption value (volume)
                LastConsumptionValue = m.Consumptions.OrderByDescending(c => c.Date).FirstOrDefault()?.Volume,

                // Get last consumption date
                LastConsumptionDate = m.Consumptions.OrderByDescending(c => c.Date).FirstOrDefault()?.Date
            }),

            // Project pending meter requests into the view model
            PendingMeterRequests = pendingMeterRequests.Select(r => new MeterRequestViewModel
            {
                Id = r.Id,
                Name = r.RequesterName,
                Email = r.RequesterEmail,
                NIF = r.NIF,
                Address = r.Address,
                Phone = r.Phone,
                RequestDate = r.RequestDate,
                Status = r.Status.ToString()
            })
        };

        // Return the view with the populated dashboard view model
        return View(viewModel);
    }


    /// <summary>
    /// Approves a pending meter request by activating the corresponding meter.
    /// </summary>
    /// <param name="id">The ID of the meter to approve.</param>
    /// <returns>Redirects to the dashboard view after approval.</returns>
    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        // Get the meter by its ID
        var meter = await _meterRepository.GetByIdAsync(id);

        // If meter does not exist, return 404
        if (meter == null)
            return NotFound();

        // Mark the meter as active and approved
        meter.IsActive = true;
        meter.Status = MeterStatus.Approved;

        // Update the meter in the database
        await _meterRepository.UpdateAsync(meter);

        // Show a success message
        TempData["StatusMessage"] = "Meter request approved successfully.";

        // Redirect to the dashboard
        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Rejects a pending meter request by deleting the corresponding meter.
    /// </summary>
    /// <param name="id">The ID of the meter to reject.</param>
    /// <returns>Redirects to the dashboard view after rejection.</returns>
    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        // Get the meter by its ID
        var meter = await _meterRepository.GetByIdAsync(id);

        // If meter does not exist, return 404
        if (meter == null)
            return NotFound();

        // Delete the meter (reject the request)
        await _meterRepository.DeleteAsync(id);

        // Show rejection message
        TempData["StatusMessage"] = "Meter request was rejected.";

        // Redirect to the dashboard
        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Deletes a meter if it has no consumption records.
    /// </summary>
    /// <param name="id">The ID of the meter to delete.</param>
    /// <returns>
    /// Redirects to the dashboard view with a status message indicating
    /// success or failure based on whether the meter had consumptions.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        // Retrieve the meter to be deleted
        var meter = await _meterRepository.GetByIdAsync(id);

        // If meter does not exist, return 404
        if (meter == null)
        {
            return NotFound();
        }

        // Check if the meter has any associated consumption records
        bool hasConsumptions = meter.Consumptions != null && meter.Consumptions.Any();

        // If consumptions exist, prevent deletion and show error
        if (hasConsumptions)
        {
            TempData["StatusMessage"] = "Cannot delete meter because it has consumption records.";
            return RedirectToAction(nameof(Index));
        }

        // Delete the meter from the system
        await _meterRepository.DeleteAsync(id);

        // Show success message
        TempData["StatusMessage"] = "Meter deleted successfully.";

        // Redirect to the dashboard
        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Displays a list of notifications for employees.
    /// </summary>
    /// <returns>The view with employee-specific notifications.</returns>
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Notifications()
    {
        // Get all notifications relevant to employees
        var notifications = await _notificationRepository.GetEmployeeNotificationsAsync();

        // Return the notifications view with the fetched list
        return View(notifications);
    }


}
