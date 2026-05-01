using Azure.Core;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Follows;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Models.Enums;
using TallahasseePRs.Api.Models.Users;
using TallahasseePRs.Api.Services.Notifications;

namespace TallahasseePRs.Api.Services.FollowServices
{
    public class FollowService : IFollowService
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        public FollowService(AppDbContext appDbContext, INotificationService notificationService)
        {
            _db = appDbContext;
            _notificationService = notificationService;
        }

        public async Task<FollowResponse> FollowAsync(Guid UserId, FollowRequest request)
        {
            var followedExists = await _db.Users.AnyAsync(l => l.Id == request.FollowedId);
            var actor = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == UserId);

            if (actor is null)
                throw new InvalidOperationException("Current user not found.");

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

            await _notificationService.CreateAsync(
                recipientId: request.FollowedId,
                actorId: UserId,
                type: NotificationType.Follow,
                message: $"{actor.UserName} started following you."
            );

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

        public async Task<int> GetFollowersCountAsync(Guid? userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
                throw new InvalidOperationException("Current user not found.");

            int count = await _db.Follows.CountAsync(f => f.FollowedId == userId);

            return count;

        }
        public async Task<int> GetFollowingCountAsync(Guid? userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("Current user not found.");

            int count = await _db.Follows.CountAsync(f => f.FollowerId == userId);

            return count;
        }

        public async Task<List<FollowResponse>> GetFollowersByUserAsync(Guid? userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("Current user not found.");

            return await _db.Follows
                .Where(a => a.FollowedId == userId)
                .Select(f => new FollowResponse //Creates new PostResponse with the selection
                {
                    Id = f.Id,
                    FollowedId = f.FollowedId,
                    FollowerId = f.FollowerId,
                    FollowedAt = f.CreatedAt

                }).ToListAsync();

        }
        public async Task<List<FollowResponse>> GetFollowingByUserAsync(Guid? userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("Current user not found.");

            return await _db.Follows
                .Where(a => a.FollowerId == userId)
                .Select(f => new FollowResponse 
                {
                    Id = f.Id,
                    FollowedId = f.FollowedId,
                    FollowerId = f.FollowerId,
                    FollowedAt = f.CreatedAt

                }).ToListAsync();

        }
    }
}
