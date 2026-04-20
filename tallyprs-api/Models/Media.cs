using TallahasseePRs.Api.Models.Posts;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.Models
{
    public enum MediaKind
    {
        Image = 1, Video = 2
    }
    public enum MediaStatus
    {
        Pending = 1,
        Processing = 2,
        Ready = 3,
        Failed = 4,
        Deleted = 5
    }
    public enum MediaPurpose
    {
        Post = 1,  Comment = 2, ProfilePicture = 3
    }
    public class Media
    {
        public Guid Id { get; set; }
        
        public Guid OwnerId { get; set; }
        public User Owner { get; set; } = null!;

        //Attachment targets
        public Guid? PostId { get; set; }
        public PRPost? Post { get; set; }
        
        public Guid? CommentId { get; set; }
        public Comment? Comment { get; set; }

        public Guid? ProfileId { get; set; }
        public Profile? Profile { get; set; }

        //Media type
        public MediaKind Kind { get; set; }
        public MediaPurpose Purpose { get; set; }
        public MediaStatus Status { get; set; } = MediaStatus.Pending;

        //Storage Identity
        public string StorageProvider { get; set; } = "cloudfare-r2";
        public string Bucket {  get; set; } = string.Empty;



        public string ObjectKey { get; set; } = string.Empty;
        public string? PlaybackObjectKey { get; set; }
        public string? ThumbnailObjectKey { get; set; }
        public string? PlaybackContentType { get; set; }




        //Metadata
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType {  get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? DurationSeconds { get; set; }

        //Hashing
        public string? Sha256Hash { get; set; }
        public string? ETag { get; set; }

        public string? BlurHash { get; set; }

        //UI
        public bool IsPublic { get; set; }
        public int SortOrder { get; set; }


        public string? ProcessingError { get; set; }

        public DateTime? ProcessingStartedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UploadedAt {  get; set; }
        public DateTime? UpdatedAt {  get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
