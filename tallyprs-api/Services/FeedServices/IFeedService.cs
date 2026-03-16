using TallahasseePRs.Api.DTOs.Feed;
using TallahasseePRs.Api.DTOs.Posts;

namespace TallahasseePRs.Api.Services.FeedServices
{
    public interface IFeedService
    {
        Task<FeedPage<PostResponse>> GetFeedAsync(FeedQuery query, Guid requestingUser);
    }
}
