using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.Models.Users;
using Microsoft.AspNetCore.Identity;
using TallahasseePRs.Api.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using TallahasseePRs.Api.Security;
using System.Threading.Tasks;


namespace TallahasseePRs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class AuthController(IAuthService authService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("Username already exists.");
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
                return BadRequest("Invalid username or password");
            return Ok(token);
    
        }

   

    }
}
