namespace TallahasseePRs.Api.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int RecipientId { get; set; }
        public MessageType Type { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User User { get; set; } = null!;


    }
}
