using Microsoft.Identity.Client;

namespace TallahasseePRs.Api.DTOs.Feed
{
    public sealed class FeedQuery
    {
        public FeedType Type { get; init; }  = FeedType.Global;

        public int Limit { get; init; } = 20;

        public DateTime? CursorCreatedAt { get; init; } 
        public Guid? CursorId { get; init; }

        public Guid? TargetUserId { get; init; }

    }
}
