namespace TallahasseePRs.Api.DTOs.Messages
{
    public class ConversationListItemResponse
    {
        public Guid Id { get; set; }

        public ConversationUserResponse? OtherUser { get; set; }

        public string? LastMessageBody { get; set; }
        public DateTime? LastMessageAtUtc { get; set; }

        public int UnreadCount { get; set; }
    }
}
