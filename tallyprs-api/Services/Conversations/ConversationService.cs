using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Messages;
using TallahasseePRs.Api.Models.Messages;
using TallahasseePRs.Api.Services.Media;
using TallahasseePRs.Api.Services.Notifications;
using TallahasseePRs.Api.Services.ProfileServices;
using TallahasseePRs.Api.Services.Storage;

namespace TallahasseePRs.Api.Services.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        private readonly IProfileService _profiles;
        private readonly IObjectStorage _storage;



        public ConversationService(AppDbContext appDbContext, INotificationService notificationService, IProfileService profiles, IObjectStorage storage)
        {
            _db = appDbContext;
            _notificationService = notificationService;
            _profiles = profiles;
            _storage = storage;
        }
        public async Task<ConversationDetailsResponse> GetConversationDetailsForUser(Guid currentUserId, Guid conversationId)
        {
            var isParticipant = await _db.ConversationParticipants.AnyAsync(x =>
               x.ConversationId == conversationId &&
               x.UserId == currentUserId);

            if (!isParticipant)
            {
                throw new InvalidOperationException("Not a participant");
            }

            Guid? otherUserId = await _db.ConversationParticipants
                .Where(cp => cp.ConversationId == conversationId && cp.UserId != currentUserId)
                .Select(cp => (Guid?)cp.UserId)
                .FirstOrDefaultAsync();

            if (otherUserId == null)
            {
                throw new InvalidOperationException("Other participant does not exist");
            }

            var otherProfile = await _profiles.GetByIdAsync(otherUserId.Value);
            if (otherProfile == null)
            {
                throw new InvalidOperationException("Other participant profile does not exist");
            }
            var otherUser = new ConversationUserResponse
            {
                UserId = otherProfile.UserId,
                DisplayName = otherProfile.DisplayName,
                ProfilePictureUrl = otherProfile.ProfilePicture?.Url ?? null
            };

            var messages = await _db.Messages
                .AsNoTracking()
                .Where(m => m.ConversationId == conversationId)
                .OrderBy(m => m.CreatedAtUtc)
                .Select(m => new MessageResponse
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    Body = m.Body,
                    SentAtUtc = m.CreatedAtUtc
                })
                .ToListAsync();

            await MarkMessagesRead(currentUserId, conversationId);

            return new ConversationDetailsResponse
            {
                Id = conversationId,
                OtherUser = otherUser,
                Messages = messages
            };

        }

        public async Task<ConversationResponse> CreateConversationAsync(Guid currentUserId, Guid otherUserId)
        {
            if (otherUserId == currentUserId)
            {
                throw new InvalidOperationException("Cannot Conversate with Self");

            }

            var otherUserExists = await _db.Profiles.AnyAsync(x =>
                x.UserId == otherUserId);

            if (!otherUserExists)
            {
                throw new InvalidOperationException("User not found.");

            }

            var existingConversationId = await _db.ConversationParticipants
                .Where(x => x.UserId == currentUserId || x.UserId == otherUserId)
                .GroupBy(x => x.ConversationId)
                .Where(g =>
                    g.Count() == 2 &&
                    g.Any(x => x.UserId == currentUserId) &&
                    g.Any(x => x.UserId == otherUserId))
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            if (existingConversationId != Guid.Empty)
            {
                var existingConversation = await _db.Conversations
                    .AsNoTracking()
                    .Where(x => x.Id == existingConversationId)
                    .Select(x => new ConversationResponse
                    {
                        Id = x.Id,
                        CreatedAtUtc = x.CreatedAtUtc
                    })
                    .FirstAsync();

                return new ConversationResponse
                {
                    Id = existingConversation.Id,
                    CreatedAtUtc = existingConversation.CreatedAtUtc
                };

            }

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = DateTime.UtcNow,
                Participants = new List<ConversationParticipant>
            {
                new ConversationParticipant
                {
                    UserId = currentUserId
                },
                new ConversationParticipant
                {
                    UserId = otherUserId
                }
            }
            };

            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();

            return new ConversationResponse
            {
                Id = conversation.Id,
                CreatedAtUtc = conversation.CreatedAtUtc
            };

        }

        public async Task<List<ConversationListItemResponse>> GetConversationsForUser(Guid currentUserId)
        {
            var conversations = await _db.Conversations
                .AsNoTracking()
                .Where(c => c.Participants.Any(p => p.UserId == currentUserId))
                .Select(c => new
                {
                    c.Id,

                    OtherUser = c.Participants
                        .Where(p => p.UserId != currentUserId)
                        .Select(p => new
                        {
                            p.UserId,
                            p.Profile.DisplayName,
                            ProfilePictureObjectKey = p.Profile.ProfilePicture != null
                                ? p.Profile.ProfilePicture.ObjectKey
                                : null
                        })
                        .FirstOrDefault(),

                    LastMessageBody = c.Messages
                        .OrderByDescending(m => m.CreatedAtUtc)
                        .Select(m => m.Body)
                        .FirstOrDefault(),

                    LastMessageAtUtc = c.Messages
                        .OrderByDescending(m => m.CreatedAtUtc)
                        .Select(m => (DateTime?)m.CreatedAtUtc)
                        .FirstOrDefault(),

                    UnreadCount = c.Messages.Count(m =>
                        m.SenderId != currentUserId &&
                        m.ReadAtUtc == null)
                })
                .OrderByDescending(c => c.LastMessageAtUtc)
                .ToListAsync();

            return conversations
                .Select(c => new ConversationListItemResponse
                {
                    Id = c.Id,

                    OtherUser = c.OtherUser is null
                        ? null
                        : new ConversationUserResponse
                        {
                            UserId = c.OtherUser.UserId,
                            DisplayName = c.OtherUser.DisplayName,
                            ProfilePictureUrl =
                                c.OtherUser.ProfilePictureObjectKey is not null
                                    ? _storage.GetPublicUrl(
                                        c.OtherUser.ProfilePictureObjectKey)
                                    : null
                        },

                    LastMessageBody = c.LastMessageBody,
                    LastMessageAtUtc = c.LastMessageAtUtc,
                    UnreadCount = c.UnreadCount
                })
                .ToList();
        }

        private async Task MarkMessagesRead(Guid currentUserId, Guid conversationId)
        {
            var now = DateTime.UtcNow;

            await _db.Messages
                .Where(m =>
                    m.ConversationId == conversationId &&
                    m.SenderId != currentUserId &&
                    m.ReadAtUtc == null)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.ReadAtUtc, now));


        }


    }
}
