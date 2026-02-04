namespace TallahasseePRs.Api.Models.Users
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User FollowerUser { get; set; } = null!;
        public User FollowedUser { get; set; } = null!;
    }
}
