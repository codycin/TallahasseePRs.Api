using TallahasseePRs.Api.DTOs.Notification;
using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.Services.Notifications
{
    public interface INotificationService
    {
        Task CreateAsync(
        Guid recipientId,
        Guid? actorId,
        NotificationType type,
        string message,
        Guid? postId = null,
        Guid? commentId = null);

        Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(Guid userId);
        Task<NotificationResponse?> MarkReadAsync(Guid userId, Guid notificationId, bool isRead);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task MarkAllReadAsync(Guid userId);

    }
}
