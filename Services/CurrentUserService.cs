using Microsoft.Identity.Client;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace TallahasseePRs.Api.Services
{
  
        public sealed class CurrentUserService : ICurrentUserService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
            }
        public Guid GetUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User
                       ?? throw new UnauthorizedAccessException("No user context.");

            var raw = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                      ?? throw new UnauthorizedAccessException("Missing NameIdentifier claim.");

            if (!Guid.TryParse(raw, out var userId))
                throw new UnauthorizedAccessException("Invalid user id claim.");

            return userId;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;
            public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;
            public Guid? UserId
            {
                get
                {
                    var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                    return Guid.TryParse(id, out var guid) ? guid : null;
                }
            }

            public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;


            
        }

    
}
