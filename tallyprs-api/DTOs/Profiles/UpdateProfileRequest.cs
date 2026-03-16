using System.ComponentModel.DataAnnotations;

namespace TallahasseePRs.Api.DTOs.Profiles
{
    public class UpdateProfileRequest
    {
        

        [MaxLength(50)]
        public string? DisplayName { get; init; }
        public Guid? ProfilePictureId { get; init; }
        public bool RemoveProfilePicture { get; init; }

        [MaxLength(80)]
        public string? HomeGym { get; init; }

        [MaxLength(40)]
        public string? LifterType { get; init; }

        [MaxLength(120)]
        public string? SpecialtyLifts { get; init; }
        public string? MeasurementsJson { get; init; }
    }
}
