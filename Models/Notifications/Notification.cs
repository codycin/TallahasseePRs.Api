using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Notifications
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public MessageType Type { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;


    }
}
