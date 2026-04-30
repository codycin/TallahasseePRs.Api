using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.Data.Configurations;
using TallahasseePRs.Api.Models;
using TallahasseePRs.Api.Services.Storage;


namespace TallahasseePRs.Api.Services.Media
{
    public class VideoProcessingService : IVideoProcessingService
    {
        private readonly AppDbContext _db;
        private readonly IObjectStorage _storage;
        private readonly VideoProcessingOptions _options;
        private readonly ILogger<VideoProcessingService> _logger;

        public VideoProcessingService(
            AppDbContext db,
            IObjectStorage storage,
            IOptions<VideoProcessingOptions> options,
            ILogger<VideoProcessingService> logger)
        {
            _db = db;
            _storage = storage;
            _options = options.Value;
            _logger = logger;
        }

        public async Task ProcessAsync(Guid mediaId, CancellationToken cancellationToken = default)
        {
            var media = await _db.Media.FirstOrDefaultAsync(m => m.Id == mediaId, cancellationToken);

            if (media is null)
                throw new InvalidOperationException("Media not found.");

            if (media.Kind != MediaKind.Video)
                throw new InvalidOperationException("Media is not a video.");

            if (media.Status == MediaStatus.Ready || media.Status == MediaStatus.Deleted)
                throw new InvalidOperationException("Media cannot be processed in its current state.");

            if (media.Status == MediaStatus.Pending)
            {
                media.Status = MediaStatus.Processing;
                media.ProcessingStartedAt = DateTime.UtcNow;
                media.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
            }

            var tempRoot = Path.Combine(Path.GetTempPath(), "tallahasseeprs-video", media.Id.ToString());
            Directory.CreateDirectory(tempRoot);

            var inputPath = Path.Combine(tempRoot, "input" + GetExtensionFromContentType(media.ContentType));
            var outputPath = Path.Combine(tempRoot, "playback.mp4");
            var thumbnailPath = Path.Combine(tempRoot, "thumbnail.jpg");

            try
            {
                await using (var inputStream = await _storage.OpenReadAsync(media.ObjectKey, cancellationToken))
                await using (var fileStream = File.Create(inputPath))
                {
                    await inputStream.CopyToAsync(fileStream, cancellationToken);
                }

                var startedAt = DateTime.UtcNow;

                _logger.LogInformation(
                    "Video processing started. MediaId={MediaId}",
                    mediaId);
                await RunFfmpegForPlaybackAsync(inputPath, outputPath, cancellationToken);
                _logger.LogInformation(
                    "Video playback file created. MediaId={MediaId}",
                    media.Id);

                Console.WriteLine("Starting thumbnail generation...");
                await RunFfmpegForThumbnailAsync(inputPath, thumbnailPath, cancellationToken);
                Console.WriteLine("Thumbnail generation finished.");

                var metadata = await ProbeVideoAsync(outputPath, cancellationToken);

                var playbackObjectKey = BuildPlaybackObjectKey(media);
                var thumbnailObjectKey = BuildThumbnailObjectKey(media);

                await using (var playbackStream = File.OpenRead(outputPath))
                {
                    await _storage.UploadViaPresignedUrlAsync(
                        playbackStream,
                        playbackObjectKey,
                        "video/mp4",
                        cancellationToken);
                }

                await using (var thumbnailStream = File.OpenRead(thumbnailPath))
                {
                     await _storage.UploadViaPresignedUrlAsync(
                         thumbnailStream,
                         thumbnailObjectKey,
                         "image/jpeg",
                         cancellationToken);
                }

                media.PlaybackObjectKey = playbackObjectKey;
                media.PlaybackContentType = "video/mp4";
                media.ThumbnailObjectKey = thumbnailObjectKey;

                media.Width = metadata.Width;
                media.Height = metadata.Height;
                media.DurationSeconds = metadata.DurationSeconds;

                media.Status = MediaStatus.Ready;
                media.ProcessedAt = DateTime.UtcNow;
                media.UpdatedAt = DateTime.UtcNow;
                media.ProcessingError = null;

                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "Video processing completed. MediaId={MediaId} DurationMs={DurationMs} Width={Width} Height={Height} DurationSeconds={DurationSeconds}",
                    media.Id,
                    (DateTime.UtcNow - startedAt).TotalMilliseconds,
                    media.Width,
                    media.Height,
                    media.DurationSeconds);
            }
            catch (Exception ex)
            {
                media.Status = MediaStatus.Failed;
                media.ProcessingError = ex.Message;
                media.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(cancellationToken);
                _logger.LogError(
                    ex,
                    "Video processing failed. MediaId={MediaId}",
                    mediaId);
                throw;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempRoot))
                        Directory.Delete(tempRoot, recursive: true);
                }
                catch
                {
                }
            }
        }

        private async Task RunFfmpegForPlaybackAsync(string inputPath, string outputPath, CancellationToken cancellationToken)
        {
            var args =
                $"-y -i \"{inputPath}\" " +
                $"-vf \"scale='min({_options.MaxPlaybackWidth},iw)':'min({_options.MaxPlaybackHeight},ih)':force_original_aspect_ratio=decrease\" " +
                "-c:v libx264 -preset medium -crf 23 " +
                "-c:a aac -movflags +faststart " +
                $"\"{outputPath}\"";

            await RunProcessAsync(_options.FfmpegPath, args, cancellationToken);
        }

        private async Task RunFfmpegForThumbnailAsync(string inputPath, string thumbnailPath, CancellationToken cancellationToken)
        {
            var args =
                $"-y -ss {_options.ThumbnailSecond} -i \"{inputPath}\" -vframes 1 " +
                $"\"{thumbnailPath}\"";

            await RunProcessAsync(_options.FfmpegPath, args, cancellationToken);
        }

        private async Task<VideoProbeResult> ProbeVideoAsync(string filePath, CancellationToken cancellationToken)
        {
            var args = $"-v quiet -print_format json -show_streams -show_format \"{filePath}\"";

            var output = await RunProcessForOutputAsync(_options.FfprobePath, args, cancellationToken);

            using var doc = JsonDocument.Parse(output);

            var streams = doc.RootElement.GetProperty("streams");
            var videoStream = streams.EnumerateArray()
                .FirstOrDefault(x => x.TryGetProperty("codec_type", out var codecType) && codecType.GetString() == "video");

            int? width = null;
            int? height = null;

            if (videoStream.ValueKind != JsonValueKind.Undefined)
            {
                if (videoStream.TryGetProperty("width", out var widthEl))
                    width = widthEl.GetInt32();

                if (videoStream.TryGetProperty("height", out var heightEl))
                    height = heightEl.GetInt32();
            }

            double? duration = null;
            if (doc.RootElement.TryGetProperty("format", out var formatEl) &&
                formatEl.TryGetProperty("duration", out var durationEl) &&
                double.TryParse(durationEl.GetString(), out var parsedDuration))
            {
                duration = parsedDuration;
            }

            return new VideoProbeResult
            {
                Width = width,
                Height = height,
                DurationSeconds = duration
            };
        }

        private async Task RunProcessAsync(string fileName, string arguments, CancellationToken cancellationToken)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi)
                    ?? throw new InvalidOperationException($"Failed to start process: {fileName}");

                var stdOutTask = process.StandardOutput.ReadToEndAsync();
                var stdErrTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync(cancellationToken);

                var stdOut = await stdOutTask;
                var stdErr = await stdErrTask;

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"Process '{fileName}' failed with exit code {process.ExitCode}. Output: {stdOut} Error: {stdErr}");
                }
            }
            catch (Win32Exception ex)
            {
                throw new InvalidOperationException(
                    $"Could not start '{fileName}'. Make sure the configured path is correct.",
                    ex);
            }
        }

        private async Task<string> RunProcessForOutputAsync(string fileName, string arguments, CancellationToken cancellationToken)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi)
                    ?? throw new InvalidOperationException($"Failed to start process: {fileName}");

                var stdOutTask = process.StandardOutput.ReadToEndAsync();
                var stdErrTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync(cancellationToken);

                var stdOut = await stdOutTask;
                var stdErr = await stdErrTask;

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"Process '{fileName}' failed with exit code {process.ExitCode}. Error: {stdErr}");
                }

                return stdOut;
            }
            catch (Win32Exception ex)
            {
                throw new InvalidOperationException(
                    $"Could not start '{fileName}'. Make sure the configured path is correct.",
                    ex);
            }
        }

        private static string GetExtensionFromContentType(string contentType)
        {
            return contentType.ToLowerInvariant() switch
            {
                "video/mp4" => ".mp4",
                "video/webm" => ".webm",
                "video/quicktime" => ".mov",
                _ => ".bin"
            };
        }

        private static string BuildPlaybackObjectKey(Models.Media media)
        {
            return media.Purpose switch
            {
                MediaPurpose.Post => media.PostId.HasValue
                    ? $"users/{media.OwnerId}/posts/{media.PostId.Value}/media/{media.Id}/playback.mp4"
                    : $"users/{media.OwnerId}/posts/unattached/media/{media.Id}/playback.mp4",

                MediaPurpose.Comment => media.CommentId.HasValue
                    ? $"users/{media.OwnerId}/comments/{media.CommentId.Value}/media/{media.Id}/playback.mp4"
                    : $"users/{media.OwnerId}/comments/unattached/media/{media.Id}/playback.mp4",

                _ => throw new InvalidOperationException("Playback video is only supported for post/comment media.")
            };
        }

        private static string BuildThumbnailObjectKey(Models.Media media)
        {
            return media.Purpose switch
            {
                MediaPurpose.Post => media.PostId.HasValue
                    ? $"users/{media.OwnerId}/posts/{media.PostId.Value}/media/{media.Id}/thumbnail.jpg"
                    : $"users/{media.OwnerId}/posts/unattached/media/{media.Id}/thumbnail.jpg",

                MediaPurpose.Comment => media.CommentId.HasValue
                    ? $"users/{media.OwnerId}/comments/{media.CommentId.Value}/media/{media.Id}/thumbnail.jpg"
                    : $"users/{media.OwnerId}/comments/unattached/media/{media.Id}/thumbnail.jpg",

                _ => throw new InvalidOperationException("Thumbnail is only supported for post/comment media.")
            };
        }

        private sealed class VideoProbeResult
        {
            public int? Width { get; set; }
            public int? Height { get; set; }
            public double? DurationSeconds { get; set; }
        }
    }
}
