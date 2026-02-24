using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TallahasseePRs.Api.DTOs.Data;
using TallahasseePRs.Api.DTOs.Votes;
using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Posts;

namespace TallahasseePRs.Api.Services.PostServices
{
    public sealed class VoteService : IVoteService
    {
        private readonly AppDbContext _db;

        public VoteService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<VoteResponse> CreateVote(Guid userId, Guid postId, VoteValue voteValue)
        {
            var postExists = await _db.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new KeyNotFoundException("Post not found");

            var vote = new Vote
            {
                UserId = userId,
                PRPostId = postId,
                Value = voteValue,
                Id = Guid.NewGuid()
            };

            _db.Votes.Add(vote);
            await _db.SaveChangesAsync();

            return ToResponse(vote);

        }
        //Currently only gets up votes
        public async Task<List<VoteResponse>> GetVotes(Guid postId)
        {
            var postExists = await _db.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new KeyNotFoundException("Post not found");

            return await _db.Votes
                .AsNoTracking()
                .Where(v => v.PRPostId == postId && v.Value == VoteValue.Up)
                .Select(v => new VoteResponse(v.Id, v.PRPostId, v.UserId, v.CreatedAt, v.Value))
                .ToListAsync();
        }

        public async Task DeleteAsync(Guid requestingUserId, Guid voteId)
        {
            var vote = await _db.Votes.FirstOrDefaultAsync(c => c.Id == voteId);
            if (vote is null) throw new KeyNotFoundException("Comment not found.");

            if (vote.UserId != requestingUserId)
                    throw new UnauthorizedAccessException("You can only delete your own comments.");

            _db.Votes.Remove(vote);
            await _db.SaveChangesAsync();

        }

        private static VoteResponse ToResponse(Vote v) =>
            new
            (
                Id: v.Id,
                PostId: v.PRPostId,
                UserId: v.UserId,
                Value: v.Value,
                CreatedAt: v.CreatedAt
            );
    }
}
