using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface INotifyRepository
{
    public Task<Notification?> GetNotificationById(int id);
    public Task UpdateReadStatus(int notificationId);
    public Task CreateNotification(int ticketId, int userId, string type);
    public Task<IEnumerable<dynamic>> GetLatestNotifiesById(int userId);
    public Task ClearNotificationsByUserId(int userId);
}