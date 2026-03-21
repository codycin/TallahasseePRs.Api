using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Media;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.Media;

namespace TallahasseePRs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AppDbContext _db;

        public MediaController(AppDbContext db, IMediaService mediaService, ICurrentUserService currentUserService)
        {
            _mediaService = mediaService;
            _currentUserService = currentUserService;
            _db = db;
        }

        [HttpPost("uploads")]
        public async Task<ActionResult<CreateMediaUploadResponse>> CreateUpload(
            [FromBody] CreateMediaUploadRequest request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var result = await _mediaService.CreateUploadAsync(userId, request, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<ActionResult<MediaResponse>> CompleteUpload(Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var result = await _mediaService.MarkUploadCompleteAsync(id, userId, cancellationToken);

            if (result is null) return NotFound();

            return Ok(result);

        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<MediaResponse>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var result = await _mediaService.GetByIdAsync(
                userId,
                id,
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("post/{postId:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<MediaResponse>>> GetForPost(Guid postId, CancellationToken cancellationToken)
        {
            var result = await _mediaService.GetForPostAsync(postId, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            await _mediaService.DeleteAsync(userId,id, cancellationToken);

            return NoContent();
        }

        [HttpGet("{mediaId:guid}/debug")]
        public async Task<IActionResult> DebugMedia(Guid mediaId, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();

            var media = await _db.Media
                .AsNoTracking()
                .Where(m => m.Id == mediaId)
                .Select(m => new
                {
                    m.Id,
                    m.OwnerId,
                    m.ObjectKey,
                    m.Status,
                    CurrentUserId = userId
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (media is null)
                return NotFound(new { message = "No media row found at all." });

            return Ok(media);
        }
    }
}
