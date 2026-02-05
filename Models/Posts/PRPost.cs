using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Posts
{
    public class PRPost
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid LiftId { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public decimal Weight { get; set; }
        public string Unit { get; set; } = "lb";

        public PRstatus Status { get; set; } = PRstatus.Pending;

        public Guid? JudgedByAdminID { get; set; }
        public string? JudgeNote { get; set; }
        public DateTime? JudgedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Lift Lift { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
