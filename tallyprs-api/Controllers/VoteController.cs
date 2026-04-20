using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Comments;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.DTOs.Votes;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.PostServices;

namespace TallahasseePRs.Api.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize] // require JWT
    public sealed class VoteController : ControllerBase
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IVoteService _votes;

        public VoteController(IVoteService votes, ICurrentUserService currentUser)
        {
            _votes = votes;
            _currentUser = currentUser;
        }

        //POST
        [HttpPost("posts/{postId:guid}/votes")]
        public async Task<ActionResult<VoteResponse>> AddVote(
            [FromRoute] Guid postId,
            [FromBody] AddVoteRequest request)
        {
            var userId = _currentUser.GetUserId();
            var created = await _votes.CreateVote(userId, postId, request.vote);
            return Ok(created);
        }


      
        [AllowAnonymous] // optional: allow viewing without auth
        [HttpGet("posts/{postId:guid}/votes")]
        public async Task<ActionResult<List<VoteResponse>>> GetForPost([FromRoute] Guid postId)
        {
            var thread = await _votes.GetVotes(postId);
            return Ok(thread);
        }

       
        [HttpDelete("votes/{postId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userId = _currentUser.GetUserId();
            await _votes.DeleteAsync(userId,postId);
            return NoContent();
        }

    }
}
