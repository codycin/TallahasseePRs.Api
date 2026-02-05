using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using TallahasseePRs.Api.Security;


namespace TallahasseePRs.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class AuthController(IConfiguration configuration) : ControllerBase
    {
        public static User user = new();
        [HttpPost("register")]
        public ActionResult<User> Register(UserDto request)
        {
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, request.Password);
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;

            return Ok(user);
        }
        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request)
        {
            if ((user.Email != request.Email)
                || (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed))
            {
                return BadRequest("User does not exist or Password is incorrect");
            }
            string token = TokenService.CreateToken(user,configuration);

            return token;
        }

   

    }
}
