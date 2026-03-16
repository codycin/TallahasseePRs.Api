namespace TallahasseePRs.Api.DTOs.Follows
{
    public sealed class FollowResponse
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FollowedId { get; set; }
        public DateTime FollowedAt { get; set; } 
    }
}
