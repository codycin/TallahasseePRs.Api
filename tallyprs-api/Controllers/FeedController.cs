using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TallahasseePRs.Api.Common.Paging;
using TallahasseePRs.Api.DTOs.Feed;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.FeedServices;

namespace TallahasseePRs.Api.Controllers
{
    [ApiController]
    [Route("api/feed")]
    [Authorize]
    public sealed class FeedController : ControllerBase
    {
        private readonly IFeedService _feedService;
        private readonly ICurrentUserService _currentUser;

        public FeedController(
            IFeedService feedService,
            ICurrentUserService currentUser)
        {
            _feedService = feedService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [EnableRateLimiting("reads")]
        public async Task<ActionResult<FeedPage<PostResponse>>> Get(
            [FromQuery] FeedType type = FeedType.Global,
            [FromQuery] int limit = 20,
            [FromQuery] string? cursor = null)
        {
            var requestingUserId = _currentUser.GetUserId();

            DateTime? cursorCreatedAt = null;
            Guid? cursorId = null;
            
            if(CursorCodec.TryDecode(cursor, out var decodedAt, out var decodedId))
            {
                cursorCreatedAt = decodedAt;
                cursorId = decodedId;
            }

            var query = new FeedQuery
            {
                Type = type,
                Limit = limit,
                CursorCreatedAt = cursorCreatedAt,
                CursorId = cursorId
            };

            var page = await _feedService.GetFeedAsync(query, requestingUserId);

            return Ok(page);
        }



    }
}
