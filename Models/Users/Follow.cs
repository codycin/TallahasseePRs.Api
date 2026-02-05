namespace TallahasseePRs.Api.Models.Users
{
    public class Follow
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FollowedId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User FollowerUser { get; set; } = null!;
        public User FollowedUser { get; set; } = null!;
    }
}
