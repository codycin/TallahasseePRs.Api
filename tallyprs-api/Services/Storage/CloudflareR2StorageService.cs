using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TallahasseePRs.Api.Data.Configurations;
using System.Net.Http.Headers;


namespace TallahasseePRs.Api.Services.Storage
{
    public sealed class CloudflareR2StorageService : IObjectStorage
    {
        private readonly IAmazonS3 _s3;
        private readonly R2Options _options;

        public CloudflareR2StorageService(
            IAmazonS3 s3,
            IOptions<R2Options> options)
        {
            _s3 = s3;
            _options = options.Value;

            if (string.IsNullOrWhiteSpace(_options.AccountId))
                throw new InvalidOperationException("R2 AccountId is not configured.");

            if (string.IsNullOrWhiteSpace(_options.AccessKeyId))
                throw new InvalidOperationException("R2 AccessKeyId is not configured.");

            if (string.IsNullOrWhiteSpace(_options.SecretAccessKey))
                throw new InvalidOperationException("R2 SecretAccessKey is not configured.");

            if (string.IsNullOrWhiteSpace(_options.BucketName))
                throw new InvalidOperationException("R2 BucketName is not configured.");

            if (string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
                throw new InvalidOperationException("R2 PublicBaseUrl is not configured.");
        }

        public async Task<PutObjectResult> UploadAsync(
                Stream stream,
                string objectKey,
                string contentType,
                IDictionary<string, string>? metadata = null,
                CancellationToken cancellationToken = default)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type is required.", nameof(contentType));

            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = NormalizeKey(objectKey),
                InputStream = stream,
                ContentType = contentType,
                AutoCloseStream = true
            };

            if (stream.CanSeek)
            {
                request.Headers.ContentLength = stream.Length - stream.Position;
            }

            request.Headers.CacheControl = "public, max-age=31536000, immutable";

            if (metadata is not null)
            {
                foreach (var kvp in metadata)
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value is not null)
                        request.Metadata[kvp.Key] = kvp.Value;
                }
            }

            var response = await _s3.PutObjectAsync(request, cancellationToken);

            return new PutObjectResult
            {
                ObjectKey = NormalizeKey(objectKey),
                ETag = response.ETag
            };
        }

        public async Task DeleteAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            var request = new DeleteObjectRequest
            {
                BucketName = _options.BucketName,
                Key = NormalizeKey(objectKey)
            };

            await _s3.DeleteObjectAsync(request, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                return false;

            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _options.BucketName,
                    Key = NormalizeKey(objectKey)
                };

                await _s3.GetObjectMetadataAsync(request, cancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex) when (
                ex.StatusCode == System.Net.HttpStatusCode.NotFound ||
                ex.ErrorCode == "NoSuchKey" ||
                ex.ErrorCode == "NotFound")
            {
                return false;
            }
        }

        public string GetPublicUrl(string objectKey)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            if (string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
                throw new InvalidOperationException("R2 PublicBaseUrl is missing.");

            var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
            var key = NormalizeKey(objectKey);

            return $"{baseUrl}/{Uri.EscapeDataString(key).Replace("%2F", "/")}";
        }

        public Task<PresignedUploadResult> CreatePresignedUploadAsync(
            string objectKey,
            string contentType,
            TimeSpan expiresIn,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type is required.", nameof(contentType));

            if (expiresIn <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(expiresIn), "Expiry must be positive.");

            var expiresAt = DateTime.UtcNow.Add(expiresIn);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = NormalizeKey(objectKey),
                Verb = HttpVerb.PUT,
                Expires = expiresAt,
                ContentType = contentType
            };

            var url = _s3.GetPreSignedURL(request);

            return Task.FromResult(new PresignedUploadResult
            {
                Url = url,
                ExpiresAtUtc = expiresAt
            });
        }

        public async Task<Stream> OpenReadAsync(string objectKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            var response = await _s3.GetObjectAsync(_options.BucketName, NormalizeKey(objectKey), cancellationToken);
            return response.ResponseStream;
        }

        private static string NormalizeKey(string objectKey)
        {
            return objectKey.Trim().TrimStart('/');
        }


        private readonly HttpClient _httpClient = new HttpClient();

        public async Task UploadViaPresignedUrlAsync(
            Stream stream,
            string objectKey,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(objectKey))
                throw new ArgumentException("Object key is required.", nameof(objectKey));

            if (string.IsNullOrWhiteSpace(contentType))
                throw new ArgumentException("Content type is required.", nameof(contentType));

            var presigned = await CreatePresignedUploadAsync(
                objectKey,
                contentType,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            using var request = new HttpRequestMessage(HttpMethod.Put, presigned.Url);

            request.Content = new StreamContent(stream);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

            if (stream.CanSeek)
            {
                request.Content.Headers.ContentLength = stream.Length - stream.Position;
            }

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new InvalidOperationException(
                    $"Presigned upload failed with status {(int)response.StatusCode}: {body}");
            }
        }
}
}