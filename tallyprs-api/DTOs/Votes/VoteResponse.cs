using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.DTOs.Votes
{
    public sealed record VoteResponse(
        Guid Id,
        Guid PostId,
        Guid UserId,
        DateTime CreatedAt,
        VoteValue Value
        );
    
    
}
