using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    /// <summary>
    /// Repository for managing <see cref="Notification"/> entities.
    /// </summary>
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a new notification to the database.
        /// </summary>
        /// <param name="notification">The notification to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves unread notifications for a specified customer.
        /// </summary>
        /// <param name="clientId">The customer ID.</param>
        /// <returns>A list of unread notifications for the customer.</returns>
        public async Task<List<Notification>> GetUnreadNotificationsAsync(int clientId)
        {
            return await _context.Notifications
                .Where(n => n.CustomerId == clientId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Marks all unread notifications as read for a specified customer.
        /// </summary>
        /// <param name="clientId">The customer ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task MarkAllAsReadAsync(int clientId)
        {
            var notifs = await _context.Notifications
                .Where(n => n.CustomerId == clientId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifs)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves unread notifications intended for employees.
        /// </summary>
        /// <returns>A collection of unread employee notifications.</returns>
        public async Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.ForEmployee && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves unread notifications related to unassigned user accounts.
        /// </summary>
        /// <returns>A collection of unread notifications with messages about user account creation.</returns>
        public async Task<IEnumerable<Notification>> GetUnassignedAccountNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsRead && n.Message.Contains("Please create the user account"))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Counts the number of notifications matching a specified condition.
        /// </summary>
        /// <param name="predicate">The filter condition.</param>
        /// <returns>The count of notifications matching the condition.</returns>
        public async Task<int> CountAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await _context.Notifications.CountAsync(predicate);
        }
    }
}
