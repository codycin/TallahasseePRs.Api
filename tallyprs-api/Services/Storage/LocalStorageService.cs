using Microsoft.AspNetCore.Hosting;
using TallahasseePRs.Api.Services.Storage;

namespace TallahasseePRs.Api.Services.Storage
{
    public class LocalStorageService : IObjectStorage
    {
        private readonly string _rootPath;
        private readonly string _publicBaseUrl;

        public LocalStorageService(IWebHostEnvironment env, IConfiguration config)
        {
            _rootPath = Path.Combine(env.ContentRootPath, "LocalUploads");
            Directory.CreateDirectory(_rootPath);

            _publicBaseUrl = config["Storage:PublicBaseUrl"] ?? "https://localhost:5001/uploads";
        }

        public async Task<PutObjectResult> UploadAsync(
            Stream stream,
            string objectKey,
            string contentType,
            IDictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            var safeKey = objectKey.Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_rootPath, safeKey);

            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await using var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
            await stream.CopyToAsync(fileStream, cancellationToken);

            return new PutObjectResult
            {
                ObjectKey = objectKey,
                ETag = null
            };
        }

        public Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            var safeKey = objectKey.Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_rootPath, safeKey);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            var safeKey = objectKey.Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_rootPath, safeKey);

            return Task.FromResult(File.Exists(fullPath));
        }

        public string GetPublicUrl(string objectKey)
        {
            var encodedKey = string.Join("/", objectKey.Split('/').Select(Uri.EscapeDataString));
            return $"{_publicBaseUrl}/{encodedKey}";
        }

        public Task<PresignedUploadResult> CreatePresignedUploadAsync(
            string objectKey,
            string contentType,
            TimeSpan expiresIn,
            CancellationToken cancellationToken = default)
        {
            // Local storage doesn't need presigned URLs, but we return something usable
            var url = GetPublicUrl(objectKey);

            return Task.FromResult(new PresignedUploadResult
            {
                Url = url,
                ExpiresAtUtc = DateTime.UtcNow.Add(expiresIn)
            });
        }
    }
}