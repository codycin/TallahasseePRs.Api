using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Follows;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Services.FollowServices
{
    public class FollowService : IFollowService
    {
        private readonly AppDbContext _db;

        public FollowService(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        public async Task<FollowResponse> FollowAsync(Guid UserId, FollowRequest request)
        {
            var followedExists = await _db.Users.AnyAsync(l => l.Id == request.FollowedId);

            if (!followedExists)
            {
                throw new InvalidOperationException("User does not exist");
            }

            if (request.FollowedId == UserId)
                throw new InvalidOperationException("You cannot follow yourself.");
           
            var alreadyFollowing = await _db.Follows
            .AnyAsync(f => f.FollowerId == UserId && f.FollowedId == request.FollowedId);

            if (alreadyFollowing)
                throw new InvalidOperationException("Already following this user.");

            var follow = new Follow
            {
                Id = Guid.NewGuid(),
                FollowerId = UserId,
                FollowedId = request.FollowedId,
            };

            _db.Follows.Add(follow);
            await _db.SaveChangesAsync();

            return ToResponse(follow);
        }

        public async Task<bool> UnFollowAsync(Guid FollowerId, Guid FollowedId)
        {
            var follow = await _db.Follows.FirstOrDefaultAsync(f =>
            f.FollowerId == FollowerId && f.FollowedId == FollowedId);

            if (follow is null) return false;

            //Removes from database
            _db.Follows.Remove(follow);
            //Save changes
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<FollowResponse?> GetByIdAsync(Guid userId, Guid followId)
        {
            //Get the post by ID
            return await _db.Follows
                .Where(f =>
                f.Id == followId &&
                f.FollowerId == userId)
                .Select(f => new FollowResponse //Creates new PostResponse with the selection
                {
                    Id = f.Id,
                    FollowedId = f.FollowedId,
                    FollowerId = f.FollowerId,
                    FollowedAt = f.CreatedAt
                  
                })
                .FirstOrDefaultAsync();
        }

        private static FollowResponse ToResponse(Follow follow) => new()
        {
            Id = follow.Id,
            FollowerId = follow.FollowerId,
            FollowedId = follow.FollowedId,
            FollowedAt = follow.CreatedAt

        };
    }
}
