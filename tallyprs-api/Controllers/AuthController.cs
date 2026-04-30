using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.Models.Users;
using TallahasseePRs.Api.Services;


namespace TallahasseePRs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("register")]
        [EnableRateLimiting("auth")]

        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            var result = await authService.RegisterAsync(request);
            if (result == null)
            {
                return BadRequest("Username or Email already exists.");
            }
            return Ok(result);
        }
        [HttpPost("login")]
        [EnableRateLimiting("auth")]

        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
                return BadRequest("Invalid username or password");
            return Ok(result);

        }
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
        {
            var result = await authService.RefreshAsync(request);
            if (result == null)
                return BadRequest("Invalid refresh token");
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<ActionResult> Logout(RefreshRequest request)
        {
            var success = await authService.LogoutAsync(request);
            if (!success)
                return BadRequest("Invalid refresh token");
            return Ok();

        }
    }
}
