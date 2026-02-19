namespace TallahasseePRs.Api.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserEmail { get; }
        bool IsAuthenticated { get; }
    }
}
