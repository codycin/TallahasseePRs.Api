using TallahasseePRs.Api.DTOs.Media;
using TallahasseePRs.Api.Services.Storage;

namespace TallahasseePRs.Api.Services.Media
{
    public sealed class MediaResponseMapper : IMediaResponseMapper
    {
        private readonly IObjectStorage _storage;

        public MediaResponseMapper(IObjectStorage storage)
        {
            _storage = storage;
        }

        public MediaResponse? ToResponse(Models.Media? media)
        {
            if (media is null)
            {
                return null;
            }

            return new MediaResponse
            {
                Id = media.Id,
                Url = _storage.GetPublicUrl(media.ObjectKey),
                ThumbnailUrl = media.ThumbnailObjectKey is not null
                    ? _storage.GetPublicUrl(media.ThumbnailObjectKey)
                    : null,

                Kind = media.Kind.ToString(),
                Purpose = media.Purpose.ToString(),

                OriginalFileName = media.OriginalFileName,
                ContentType = media.ContentType,
                SizeBytes = media.SizeBytes,

                Width = media.Width,
                Height = media.Height,
                DurationSeconds = media.DurationSeconds,

                CreatedAt = media.CreatedAt
            };
        }
    }
}
