using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Posts;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Notifications
{
    public class Notification
    {
        public Guid Id { get; set; }

        public Guid RecipientId { get; set; }
        public User Recipient { get; set; } = null!;

        public Guid? ActorId { get; set; }
        public User? Actor { get; set; }

        public Guid? PostId { get; set; }
        public PRPost? Post { get; set; }

        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public NotificationType Type { get; set; }

        public string Message { get; set; } = "";

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
