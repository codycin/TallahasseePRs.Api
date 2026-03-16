using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.DTOs.Notification
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? ActorId { get; set; }
        public string? ActorUsername { get; set; }

        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
    }
}
