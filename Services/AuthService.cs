using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.DTOs.Data;
using TallahasseePRs.Api.Models.Users;
using TallahasseePRs.Api.Security;

namespace TallahasseePRs.Api.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(LoginRequest dto)
        {

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user is null) return null;
            if ((user.Email != dto.Email)
            || (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, dto.Password)
            == PasswordVerificationResult.Failed))
            {
                return null;
            }
     

            return TokenService.CreateToken(user,configuration);
        }

        public async Task<User?> RegisterAsync(RegisterRequest dto)
        {
            if(await context.Users.AnyAsync(u=>u.Email == dto.Email))
            {
                return null; //respond in controller
            }

            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
              .HashPassword(user, dto.Password);
            user.Email = dto.Email;
            user.PasswordHash = hashedPassword;
   
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
    }
}
