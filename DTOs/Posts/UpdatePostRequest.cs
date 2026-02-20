namespace TallahasseePRs.Api.DTOs.Posts
{
    public class UpdatePostRequest
    {
        public Guid? LiftId { get; set; }

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }

        public decimal? Weight { get; set; }
        public string? Unit { get; set; }
    }
}
