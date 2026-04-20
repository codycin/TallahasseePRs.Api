
namespace TallahasseePRs.Api.Services.Storage
{
    public interface IObjectStorage
    {
        Task<PutObjectResult> UploadAsync(
           Stream stream,
            string objectKey,
            string contentType,
            IDictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken = default);

        string GetPublicUrl(string objectKey);

        Task<PresignedUploadResult> CreatePresignedUploadAsync(
            string objectKey,
            string contentType,
            TimeSpan expiresIn,
            CancellationToken cancellationToken = default);

        Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default);
        Task UploadViaPresignedUrlAsync(
            Stream stream,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken = default);
    }
    public class PutObjectResult
    {
        public string ObjectKey { get; set; } = "";
        public string? ETag { get; set; }
    }

    public class PresignedUploadResult
    {
        public string Url { get; set; } = "";
        public DateTime ExpiresAtUtc { get; set; }
    }
}
