using System.Linq.Expressions;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Repository interface for managing <see cref="Notification"/> entities.
    /// Extends the generic repository with notification-specific methods.
    /// </summary>
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        /// <summary>
        /// Adds a new notification asynchronously.
        /// </summary>
        /// <param name="notification">The notification to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddNotificationAsync(Notification notification);

        /// <summary>
        /// Retrieves unread notifications for a specific customer.
        /// </summary>
        /// <param name="clientId">The customer ID.</param>
        /// <returns>A list of unread notifications for the specified customer.</returns>
        Task<List<Notification>> GetUnreadNotificationsAsync(int clientId);

        /// <summary>
        /// Marks all unread notifications as read for a specific customer.
        /// </summary>
        /// <param name="clientId">The customer ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MarkAllAsReadAsync(int clientId);

        /// <summary>
        /// Retrieves unread notifications that are intended for employees.
        /// </summary>
        /// <returns>A collection of unread employee notifications.</returns>
        Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync();

        /// <summary>
        /// Retrieves unread notifications related to unassigned user accounts.
        /// </summary>
        /// <returns>A collection of unread notifications requesting user account creation.</returns>
        Task<IEnumerable<Notification>> GetUnassignedAccountNotificationsAsync();

        /// <summary>
        /// Counts the number of notifications matching a given predicate.
        /// </summary>
        /// <param name="predicate">A filter expression.</param>
        /// <returns>The count of notifications that satisfy the predicate.</returns>
        Task<int> CountAsync(Expression<Func<Notification, bool>> predicate);
    }
}
