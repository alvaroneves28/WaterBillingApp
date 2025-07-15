using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WaterBillingApp.Data;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(int clientId)
        {
            return await _context.Notifications
                .Where(n => n.CustomerId == clientId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(int clientId)
        {
            var notifs = await _context.Notifications
                .Where(n => n.CustomerId == clientId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifs)
                n.IsRead = true;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => n.ForEmployee && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnassignedAccountNotificationsAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsRead && n.Message.Contains("Please create the user account"))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await _context.Notifications.CountAsync(predicate);
        }
    }
}
