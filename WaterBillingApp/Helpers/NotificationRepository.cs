using Microsoft.EntityFrameworkCore;
using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
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
        }

        public async Task SaveAsync()
        {
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


    }
}
