using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.ProfileServices;

namespace TallahasseePRs.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/profiles")]
    public sealed class PublicProfileController : ControllerBase
    {
        private readonly IProfileQueryService _profiles;

        public PublicProfileController(IProfileQueryService profiles)
        {
            _profiles = profiles;
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetPublic(Guid userId)
        {
            var profile = await _profiles.GetPublicByIdAsync(userId);
            return profile is null ? NotFound() : Ok(profile);
        }
    }
}
