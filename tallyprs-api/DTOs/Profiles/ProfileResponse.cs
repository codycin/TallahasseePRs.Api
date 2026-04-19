using TallahasseePRs.Api.DTOs.Follows;
using TallahasseePRs.Api.DTOs.Media;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.DTOs.Profiles
{
    public class ProfileResponse
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public MediaResponse? ProfilePicture { get; set; }
        public string? HomeGym { get; set; }
        public string? LifterType { get; set; }
        public string? SpecialtyLifts { get; set; }
        public string? MeasurementsJson { get; set; }
        public int FollowCount { get; set; }
        public int FollowingCount { get; set; }

    }
}
