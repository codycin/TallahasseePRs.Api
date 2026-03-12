namespace TallahasseePRs.Api.DTOs.Posts
{
    public sealed class CreatePostRequest
    {
        public Guid LiftId { get; set; }

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Weight { get; set; }
        public string Unit { get; set; } = "lb";

        public List<Guid> MediaIds { get; set; } = new();
    }
}
