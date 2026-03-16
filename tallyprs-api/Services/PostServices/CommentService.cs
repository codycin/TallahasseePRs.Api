using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.DTOs.Comments;
using TallahasseePRs.Api.Models.Posts;

namespace TallahasseePRs.Api.Services.PostServices
{
    public sealed class CommentService : ICommentService
    {
        private readonly AppDbContext _db;

        public CommentService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<CommentResponse> CreateTopLevelAsync(Guid postId, Guid userId, string body)
        {
            var postExists = await _db.Posts.AnyAsync(p => p.Id == postId );
            if (!postExists)
            {
                throw new KeyNotFoundException("Post not found");
            }

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PRPostId = postId,
                body = body.Trim(),
                ParentCommentId = null,
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return ToResponse(comment, replies: new List<CommentResponse>());

        }

        public async Task<CommentResponse> CreateReplyAsync(Guid postId, Guid parentCommentId, Guid userId, string body)
        {
            var parent = await _db.Comments
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == parentCommentId);
            if (parent == null)
            {
                throw new KeyNotFoundException("Parent comment not found.");
            }
                var parentExists = await _db.Comments.AnyAsync(c => c.Id == parentCommentId);

            if (parent.PRPostId != postId)
                throw new InvalidOperationException("Parent comment does not belong to this post.");

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                PRPostId = parent.PRPostId,
                body = body.Trim(),
                ParentCommentId = parentCommentId,
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return ToResponse(comment, replies: new List<CommentResponse>());
        }

        public async Task<List<CommentResponse>> GetThreadForPostAsync(Guid postId)
        {
            var comments = await _db.Comments.AsNoTracking().Where(c => c.PRPostId == postId).OrderBy(c => c.CreatedAt).ToListAsync();

            var map = comments.ToDictionary(c => c.Id, c => new CommentResponse(
                    Id: c.Id,
                    PostId: c.PRPostId,
                    UserId: c.UserId,
                    Body: c.body,
                    CreatedAt: c.CreatedAt,
                    ParentCommentId: c.ParentCommentId,
                    Replies: new List<CommentResponse>()
                )
                
            );

            // Build tree
            var roots = new List<CommentResponse>();

            foreach (var c in comments)
            {
                var node = map[c.Id];

                if (c.ParentCommentId is null)
                {
                    roots.Add(node);
                    continue;
                }

                if (map.TryGetValue(c.ParentCommentId.Value, out var parent))
                {
                    parent.Replies.Add(node);
                }
                else
                {
                    // orphan fallback: treat as root (shouldn't happen if data is consistent)
                    roots.Add(node);
                }
            }

            return roots;
        }

        public async Task DeleteAsync(Guid commentId, Guid requestingUserId)
        {
            var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment is null) throw new KeyNotFoundException("Comment not found.");

            if (comment.UserId != requestingUserId)
                throw new UnauthorizedAccessException("You can only delete your own comments.");

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();


        }

        private static CommentResponse ToResponse(Comment c, List<CommentResponse> replies) =>
        new(
            Id: c.Id,
            PostId: c.PRPostId,
            UserId: c.UserId,
            Body: c.body,
            CreatedAt: c.CreatedAt,
            ParentCommentId: c.ParentCommentId,
            Replies: replies
        );
    }
}
