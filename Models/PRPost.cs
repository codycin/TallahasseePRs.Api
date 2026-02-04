namespace TallahasseePRs.Api.Models
{
    public class PRPost
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int LiftId { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string VideoUrl { get; set; } = "";
        public decimal weight { get; set; }
        public string Unit { get; set; } = "lb";

        public PRstatus Status { get; set; } = PRstatus.Pending;

        public int? JudgedByAdminID { get; set; }
        public string? JudgeNote { get; set; }
        public DateTime? JudgedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Lift Lift { get; set; } = null!;
    }
}
