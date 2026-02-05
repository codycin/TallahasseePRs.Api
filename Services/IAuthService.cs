using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.Models.Users;


namespace TallahasseePRs.Api.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterRequest dto);
        Task<string?> LoginAsync(LoginRequest dto);
    }
}
