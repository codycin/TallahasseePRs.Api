using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TallahasseePRs.Api.Models.Users;
using System.Security.Cryptography;

using TallahasseePRs.Api.Security;

namespace TallahasseePRs.Api.Security
{
    public class TokenService
    {
        public static (string Token, DateTime expiresAtUtc) CreateToken(User user, IConfiguration configuration)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = configuration["Jwt:Key"]!;
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];

            var minutes = int.TryParse(configuration["Jwt:AccessTokenExpirationMinutes"], out var m) ? m : 60;
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(minutes);

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512);


            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(jwt),expiresAtUtc);
        }
    }
}
