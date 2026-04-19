using TallahasseePRs.Api.DTOs.Media;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.DTOs.Profiles
{
    public class PublicProfileResponse
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public MediaResponse? ProfilePicture { get; set; }
        public string? HomeGym { get; set; }
        public string? LifterType { get; set; }
        public string? SpecialtyLifts { get; set; }
        public int FollowCount { get; set; }
        public int FollowingCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }

    }
}
