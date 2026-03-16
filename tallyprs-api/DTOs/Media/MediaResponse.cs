namespace TallahasseePRs.Api.DTOs.Media
{
    public class MediaResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = "";
        public string? ThumbnailUrl { get; set; }

        public string Kind { get; set; } = "";
        public string Purpose { get; set; } = "";
        public string ContentType { get; set; } = "";
        public string OriginalFileName { get; set; } = "";
        public long SizeBytes { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? DurationSeconds { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
