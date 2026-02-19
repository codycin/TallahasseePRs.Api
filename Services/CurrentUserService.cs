using System.Security.Claims;

namespace TallahasseePRs.Api.Services
{
  
        public sealed class CurrentUserService : ICurrentUserService
        {
            private readonly IHttpContextAccessor _httpContextAccessor;
            public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            {
                _httpContextAccessor = httpContextAccessor;
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
