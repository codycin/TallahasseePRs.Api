namespace TallahasseePRs.Api.DTOs.Messages
{
    public class ConversationUserResponse
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }
}

