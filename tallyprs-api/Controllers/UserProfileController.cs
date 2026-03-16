using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Profiles;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.PostServices;
using TallahasseePRs.Api.Services.ProfileServices;

namespace TallahasseePRs.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/me/[controller]")]
    public sealed class UserProfileController : ControllerBase
    {
        private readonly IProfileService _profiles;
        private readonly ICurrentUserService _currentUser;
        public UserProfileController(IProfileService profiles, ICurrentUserService CurrentUser)
        {
            _profiles = profiles;
            _currentUser = CurrentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var profile = await _profiles.GetByIdAsync(_currentUser.GetUserId());
            return (profile == null) ? NotFound() : Ok(profile);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request)
        {
            var userId = _currentUser.GetUserId();

            try
            {
                var updated = await _profiles.UpdateAsync(userId, request);
                if (updated is null) return NotFound();
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
