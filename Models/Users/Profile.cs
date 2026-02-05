namespace TallahasseePRs.Api.Models.Users
{
    public class Profile
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = "";
        public string? PfpUrl { get; set; }
        public string? HomeGym { get; set; }
        public string? LifterType { get; set; }
        public string? SpecialtyLifts { get; set; }
        public string? MeasurmentsJson { get; set; }
        public User User { get; set; } = null!;

    }
}
