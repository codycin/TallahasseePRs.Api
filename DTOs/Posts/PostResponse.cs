using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.DTOs.Posts
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid LiftId { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string VideoUrl { get; set; } = "";

        public decimal Weight { get; set; }
        public string Unit { get; set; } = "lb";

        public PRstatus Status { get; set; }

        public Guid? JudgedByAdminID { get; set; }
        public string? JudgeNote { get; set; }
        public DateTime? JudgedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        // optional extras that clients usually want:
        public int CommentCount { get; set; }
    }
}
