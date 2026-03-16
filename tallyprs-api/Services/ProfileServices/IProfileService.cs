using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.DTOs.Profiles;

namespace TallahasseePRs.Api.Services.ProfileServices
{
    public interface IProfileService
    {
        Task<ProfileResponse?> GetByIdAsync(Guid userId);
        Task<ProfileResponse?> UpdateAsync( Guid userId, UpdateProfileRequest request);
    }
    public interface IProfileQueryService
    {
        Task<PublicProfileResponse?> GetPublicByIdAsync(Guid userId);
    }
}
