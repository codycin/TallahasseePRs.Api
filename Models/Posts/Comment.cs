using TallahasseePRs.Api.Models.Notifications;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models.Posts
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid PRPostId { get; set; }
        public Guid UserId { get; set; }
        public string body { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? ParentCommentId { get; set; }


        public User User { get; set; } = null!;
        public PRPost PRPost { get; set; } = null!;
        public Comment? ParentComment { get; set; } 

        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Media> MediaItems { get; set; } = new List<Media>();


    }
}
