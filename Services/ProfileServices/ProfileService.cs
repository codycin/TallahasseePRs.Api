
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Profiles;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Services.ProfileServices
{
    public class ProfileService : IProfileService, IProfileQueryService
    {
        private readonly AppDbContext _db;

        public ProfileService(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }
        public async Task<ProfileResponse?> GetByIdAsync(Guid userId)
        {
            var profile = await _db.Profiles.Where(p => p.UserId == userId).SingleOrDefaultAsync();
            if (profile == null) return null;

            return ToResponse(profile);

        }
        public async Task<ProfileResponse?> UpdateAsync(Guid userId, UpdateProfileRequest request)
        {
            var profile = await _db.Profiles.Where(p => p.UserId == userId).SingleOrDefaultAsync();
            if (profile == null) return null;

            // Null = don't change semantics
            if (request.DisplayName is not null)
                profile.DisplayName = request.DisplayName.Trim();

            if (request.PfpUrl is not null)
                profile.PfpUrl = string.IsNullOrWhiteSpace(request.PfpUrl) ? null : request.PfpUrl.Trim();

            if (request.HomeGym is not null)
                profile.HomeGym = string.IsNullOrWhiteSpace(request.HomeGym) ? null : request.HomeGym.Trim();

            if (request.LifterType is not null)
                profile.LifterType = string.IsNullOrWhiteSpace(request.LifterType) ? null : request.LifterType.Trim();

            if (request.SpecialtyLifts is not null)
                profile.SpecialtyLifts = string.IsNullOrWhiteSpace(request.SpecialtyLifts) ? null : request.SpecialtyLifts.Trim();

            if (request.MeasurementsJson is not null)
            {
                profile.MeasurmentsJson = string.IsNullOrWhiteSpace(request.MeasurementsJson)
                    ? null
                    : request.MeasurementsJson;
            }

            await _db.SaveChangesAsync();

            return ToResponse(profile);

        }

        public async Task<PublicProfileResponse?> GetPublicByIdAsync(Guid userId)
        {
            //Later implement for public/private account
            var p = await _db.Profiles.SingleOrDefaultAsync(x => x.UserId == userId);
            return p is null ? null : ToPublicResponse(p);
        }

        private static ProfileResponse ToResponse(Profile profile) => new()
        {
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            PfpUrl = profile.PfpUrl,
            HomeGym = profile.HomeGym,
            LifterType = profile.LifterType,
            SpecialtyLifts = profile.SpecialtyLifts,
            MeasurementsJson = profile.MeasurmentsJson

        };
        private static PublicProfileResponse ToPublicResponse(Profile profile) => new()
        {
            UserId = profile.UserId,
            DisplayName = profile.DisplayName,
            PfpUrl = profile.PfpUrl,
            HomeGym = profile.HomeGym,
            LifterType = profile.LifterType,
            SpecialtyLifts = profile.SpecialtyLifts,
        };
    }
}
