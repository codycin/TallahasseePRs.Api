using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Comments;
using TallahasseePRs.Api.DTOs.Notification;
using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Notifications;

namespace TallahasseePRs.Api.Services.Notifications
{
    public sealed class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;

        public NotificationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task CreateAsync(
        Guid recipientId,
        Guid? actorId,
        NotificationType type,
        string message,
        Guid? postId = null,
        Guid? commentId = null)
        {
            if (actorId.HasValue && actorId.Value == recipientId)
                return;

            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = recipientId,
                ActorId = actorId,
                Type = type,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                PostId = postId,
                CommentId = commentId
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<NotificationResponse>> GetForUserAsync(Guid userId)
        {
            return await _db.Notifications
                .AsNoTracking()
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Type = n.Type,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ActorId = n.ActorId,
                    ActorUsername = n.Actor != null ? n.Actor.UserName : null,
                    PostId = n.PostId,
                    CommentId = n.CommentId
                }).ToListAsync();
        }
        public async Task<NotificationResponse?> MarkReadAsync(Guid userId, Guid notificationId, bool isRead)
        {
            var notification = await _db.Notifications
                .Include(n => n.Actor)
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId);

            if (notification is null) return null;

            notification.IsRead = isRead;

            await _db.SaveChangesAsync();

            return ToResponse(notification);
        }
        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _db.Notifications
                .AsNoTracking()
                .CountAsync(n => n.RecipientId == userId && !n.IsRead);
        }
        public async Task MarkAllReadAsync(Guid userId)
        {
            var unreadNotifications = await _db.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Count == 0)
                return;

            foreach (var n in unreadNotifications)
            {
                n.IsRead = true;
            }

            await _db.SaveChangesAsync();
        }

        private static NotificationResponse ToResponse(Notification notification) => new NotificationResponse
        {
            Id = notification.Id,
            Type = notification.Type,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ActorId = notification.ActorId,
            ActorUsername = notification.Actor?.UserName,
            PostId = notification.PostId,
            CommentId = notification.CommentId
        };
    }
}
