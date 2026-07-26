using Microsoft.AspNetCore.Mvc;
using TallahasseePRs.Api.DTOs.Messages;

namespace TallahasseePRs.Api.Services.Conversations
{
    public interface IConversationService
    {
        Task<ConversationDetailsResponse> GetConversationDetailsForUser(Guid currentUserId, Guid conversationId);
        Task<List<ConversationListItemResponse>> GetConversationsForUser(Guid currentUserId);
        Task<ConversationResponse> CreateConversationAsync(Guid currentUserId, Guid otherUserID);
    }
}
