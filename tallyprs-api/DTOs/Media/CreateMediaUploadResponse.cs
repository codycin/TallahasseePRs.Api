namespace TallahasseePRs.Api.DTOs.Media
{
    public class CreateMediaUploadResponse
    {
        public Guid MediaId { get; set; }
        public string UploadUrl { get; set; } = "";
        public string ObjectKey { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
     
    }
}
