using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.DTOs.Votes
{
    public sealed record AddVoteRequest(
        VoteValue vote
        );
    
}
