using TallahasseePRs.Api.DTOs.Votes;
using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.Services.PostServices
{
    public interface IVoteService
    {
        Task<VoteResponse> CreateVote(Guid userId, Guid postId, VoteValue voteValue);

        Task<List<VoteResponse>> GetVotes(Guid postId);

        Task DeleteAsync(Guid requestingUserId, Guid voteId);
    }
}
