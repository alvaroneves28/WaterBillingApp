using WaterBillingApp.Data.Entities;

namespace WaterBillingApp.Helpers
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetUnreadNotificationsAsync(int clientId);
        Task MarkAllAsReadAsync(int clientId);
        Task SaveAsync();

        Task<IEnumerable<Notification>> GetEmployeeNotificationsAsync();

        Task<IEnumerable<Notification>> GetUnassignedAccountNotificationsAsync();

    }
}
