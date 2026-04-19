using TallahasseePRs.Api.DTOs.Follows;

namespace TallahasseePRs.Api.Services.FollowServices
{
    public interface IFollowService
    {
        Task<FollowResponse> FollowAsync(Guid User, FollowRequest request);
        Task<FollowResponse?> GetByIdAsync(Guid userId, Guid followId);
        Task<bool> UnFollowAsync(Guid FollowerId, Guid FollowedId);

        Task<int> GetFollowersCountAsync(Guid userId);
        Task<int> GetFollowingCountAsync(Guid userId);

        Task<List<FollowResponse>> GetFollowersByUserAsync(Guid userId);
        Task<List<FollowResponse>> GetFollowingByUserAsync(Guid userId);
    }
}
