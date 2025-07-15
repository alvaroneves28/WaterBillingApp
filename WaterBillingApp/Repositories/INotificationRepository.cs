using System.Linq.Expressions;
using WaterBillingApp.Data.Entities;
using WaterBillingApp.Repositories;

namespace WaterBillingApp.Helpers
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetUnreadNotificationsAsync(int clientId);
        Task MarkAllAsReadAsync(int clientId);
        Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync();
        Task<IEnumerable<Notification>> GetUnassignedAccountNotificationsAsync();
        Task<int> CountAsync(Expression<Func<Notification, bool>> predicate);


    }
}
