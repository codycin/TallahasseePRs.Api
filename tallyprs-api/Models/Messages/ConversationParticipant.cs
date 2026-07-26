using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Messages
{
    public class ConversationParticipant
    {
        public Guid ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public Guid UserId { get; set; }

        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? LastReadAtUtc { get; set; }

        public Profile Profile { get; set; } = null!;

    }
}
