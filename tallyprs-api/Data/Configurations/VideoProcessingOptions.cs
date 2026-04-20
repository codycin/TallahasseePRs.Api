namespace TallahasseePRs.Api.Data.Configurations
{
    public class VideoProcessingOptions
    {
        public string FfmpegPath { get; set; } = "ffmpeg";
        public string FfprobePath { get; set; } = "ffprobe";
        public int MaxPlaybackWidth { get; set; } = 1280;
        public int MaxPlaybackHeight { get; set; } = 720;
        public int ThumbnailSecond { get; set; } = 1;
    }
}
