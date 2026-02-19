using TallahasseePRs.Api.DTOs.Auth;
using TallahasseePRs.Api.Models.Users;


namespace TallahasseePRs.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> RegisterAsync(RegisterRequest dto);
        Task<AuthResponse?> LoginAsync(LoginRequest dto);

        Task<AuthResponse?> RefreshAsync(RefreshRequest dto);
        Task<bool> LogoutAsync(RefreshRequest dto);
    }
}
