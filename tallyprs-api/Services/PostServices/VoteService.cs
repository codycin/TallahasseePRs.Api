using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using TallahasseePRs.Api.Data;
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

            var existingVote = await _db.Votes
                .FirstOrDefaultAsync(v => v.PRPostId == postId && v.UserId == userId);

            if (existingVote is not null)
            {
                await DeleteAsync(userId, existingVote.PRPostId);
            }

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

        public async Task DeleteAsync(Guid requestingUserId, Guid postId)
        {
            var vote = await _db.Votes.FirstOrDefaultAsync(c => c.PRPostId == postId && c.UserId == requestingUserId);
            if (vote is null) throw new KeyNotFoundException("Vote not found.");


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
