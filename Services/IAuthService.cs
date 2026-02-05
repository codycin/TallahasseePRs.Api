using TallahasseePRs.Api.DTOs.Auth;


namespace TallahasseePRs.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest dto);
        Task<AuthResponse> LoginAsync(LoginRequest dto);
    }
}
