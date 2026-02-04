using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Posts
{
    public class Comment
    {
        public Guid Id { get; set; }
        public int PRPostId { get; set; }
        public int UserId { get; set; }
        public string body { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public PRPost PRPost { get; set; } = null!;
        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; } 
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    }
}
