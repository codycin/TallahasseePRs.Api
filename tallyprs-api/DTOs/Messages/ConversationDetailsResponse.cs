namespace TallahasseePRs.Api.DTOs.Messages
{
    public class ConversationDetailsResponse
    {
        public Guid Id { get; set; }

        public ConversationUserResponse OtherUser { get; set; } = null!;

        public List<MessageResponse> Messages { get; set; } = [];
    }
}
