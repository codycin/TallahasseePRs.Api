using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Posts
{
    public class Vote
    {
        public int Id { get; set; }
        public int PRPostId { get; set; }
        public int UserId { get; set; }
        public VoteValue Value { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public PRPost PRPost { get; set; } = null!;
    }
}
