using TallahasseePRs.Api.Models;

namespace TallahasseePRs.Api.DTOs.Media
{
    public class CreateMediaUploadRequest
    {
        public string FileName { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long SizeBytes { get; set; }

        public MediaPurpose Purpose { get; set; } 
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public Guid? ProfileId { get; set; }
    }
}
