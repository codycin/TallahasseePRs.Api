using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Messages;
using TallahasseePRs.Api.Services;
using TallahasseePRs.Api.Services.Conversations;

namespace TallahasseePRs.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/conversations")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversation;
        private readonly ICurrentUserService _currentUser;

        public ConversationsController(IConversationService conversation, ICurrentUserService currentUser)
        {
            _conversation = conversation;
            _currentUser = currentUser;
        }

        [HttpGet("{conversationId}")]
        public async Task<ActionResult<ConversationDetailsResponse>> GetMessages(Guid conversationId)
        {
            var currentUserId = _currentUser.GetUserId();

            return Ok(await _conversation.GetConversationDetailsForUser(currentUserId, conversationId));
           
        }
        [HttpGet]
        public async Task<ActionResult<List<ConversationListItemResponse>>> GetConversations()
        {
            var currentUser = _currentUser.GetUserId();
           
            return Ok(await _conversation.GetConversationsForUser(currentUser));
        }

        [HttpPost]
        public async Task<ActionResult<ConversationResponse>> CreateConversation(
        CreateConversationRequest request)
        {
            var currentUserId = _currentUser.GetUserId();

            try
            {  
                var conversation = await _conversation.CreateConversationAsync(currentUserId, request.OtherUserId);
                return Ok(conversation);
            }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (UnauthorizedAccessException) { return Forbid(); }

        }
    }
}
