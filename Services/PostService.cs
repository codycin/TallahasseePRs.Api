using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.DTOs.Data;
using TallahasseePRs.Api.DTOs.Posts;
using TallahasseePRs.Api.Models.Posts;
using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.Services
{
    public sealed class PostService : IPostService
    {
        private readonly AppDbContext _db;

        public PostService(AppDbContext appDbContext)
        {
            _db = appDbContext;
        }

        //Create post
        public async Task<PostResponse> CreateAsync(Guid userId, CreatePostRequest request)
        {
            //Check if lift exists through searching database using await, passing table
            //any async, search for every lifts id compare to LiftId from the Create post Request
            var liftExists = await _db.Lifts.AnyAsync(l => l.Id == request.LiftId);

            if (!liftExists)
            {
                throw new InvalidOperationException("LiftID does not exist");

            }
            //Now we create the new post
            var post = new PRPost
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LiftId = request.LiftId,

                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                VideoUrl = request.VideoUrl.Trim(),

                Weight = request.Weight,
                Unit = string.IsNullOrWhiteSpace(request.Unit) ? "lb" : request.Unit.Trim(),

                Status = PRstatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            //Now we have to actually add it to database with this pattern
            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            return ToResponse(post, commentCount: 0);

        }

        public async Task<PostResponse?> GetByIdAsync(Guid postId)
        {
            //Get the post by ID
            return await _db.Posts
                .Where(p => p.Id == postId)
                .Select(p => new PostResponse //Creates new PostResponse with the selection
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    LiftId = p.LiftId,
                    Title = p.Title,
                    Description = p.Description,
                    VideoUrl = p.VideoUrl,
                    Weight = p.Weight,
                    Unit = p.Unit,
                    Status = p.Status,
                    JudgedByAdminID = p.JudgedByAdminID,
                    JudgeNote = p.JudgeNote,
                    JudgedAt = p.JudgedAt,
                    CreatedAt = p.CreatedAt,
                    CommentCount = p.Comments.Count
                })
                .FirstOrDefaultAsync();
        }
        //Update posts
        public async Task<PostResponse?> UpdateAsync(Guid userId, Guid postId, UpdatePostRequest request)
        {
            //Fetch ID and check
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post is null) return null;

            // Make sure User is trying to edit post
            if (post.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this post.");

            //Checks that new LiftId infact does exist
            if (request.LiftId.HasValue)
            {
                var liftExists = await _db.Lifts.AnyAsync(l => l.Id == request.LiftId.Value);
                if (!liftExists)
                    throw new InvalidOperationException("LiftId does not exist.");

                post.LiftId = request.LiftId.Value;
            }
            //Checks if anything else was updated
            if (request.Title is not null) post.Title = request.Title.Trim();
            if (request.Description is not null) post.Description = request.Description.Trim();
            if (request.VideoUrl is not null) post.VideoUrl = request.VideoUrl.Trim();
            if (request.Weight.HasValue) post.Weight = request.Weight.Value;
            if (request.Unit is not null) post.Unit = string.IsNullOrWhiteSpace(request.Unit) ? post.Unit : request.Unit.Trim();

            //Save changed to database
            await _db.SaveChangesAsync();

            //Saves comment count
            var commentCount = await _db.Comments.CountAsync(c => c.Id == postId); //THIS WAS WEIRD, CHECK AGAIN FOR COMMENT FUNCTION
            return ToResponse(post, commentCount);
        }
        public async Task<bool> DeleteAsync(Guid userId, Guid postId)
        {
            //Fetches post and makes sure it exists
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post is null) return false;

            //Makes sure owner is owner
            if (post.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this post.");

            //Removes from database
            _db.Posts.Remove(post);
            //Save changes
            await _db.SaveChangesAsync();
            return true;
        }

        private static PostResponse ToResponse(PRPost p, int commentCount) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            LiftId = p.LiftId,
            Title = p.Title,
            Description = p.Description,
            VideoUrl = p.VideoUrl,
            Weight = p.Weight,
            Unit = p.Unit,
            Status = p.Status,
            JudgedByAdminID = p.JudgedByAdminID,
            JudgeNote = p.JudgeNote,
            JudgedAt = p.JudgedAt,
            CreatedAt = p.CreatedAt,
            CommentCount = commentCount
        };
    }

}
