using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using TallahasseePRs.Api.DTOs.Notification;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.Notifications;

namespace TallahasseePRs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notifications;
        private readonly ICurrentUserService _currentUser;

        public NotificationsController(INotificationService notifications, ICurrentUserService currentUser)
        {
            _notifications = notifications;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetMine()
        {
            var userId = _currentUser.GetUserId();
            var result = await _notifications.GetForUserAsync(userId);
            return Ok(result);
        }
        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<NotificationResponse>> UpdateReadStatus(Guid id, [FromBody] NotificationReadRequest req)
        {
            var userId = _currentUser.GetUserId();
            var result = await _notifications.MarkReadAsync(userId, id, req.IsRead);

            if(result is null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var userId = _currentUser.GetUserId();
            var count = await _notifications.GetUnreadCountAsync(userId); 
            return Ok(count);
        }

        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = _currentUser.GetUserId();
            await _notifications.MarkAllReadAsync(userId);
            return Ok();
        }
    }
}
