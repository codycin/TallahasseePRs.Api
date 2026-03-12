namespace TallahasseePRs.Api.Data.Configurations
{
    public class MediaOptions
    {
        public string BucketName { get; set; } = "";
        public string PublicBaseUrl { get; set; } = "";

        public long MaxAvatarBytes { get; set; } = 5 * 1024 * 1024;
        public long MaxImageBytes { get; set; } = 10 * 1024 * 1024;
        public long MaxVideoBytes { get; set; } = 100 * 1024 * 1024;

        public int MaxPostVideoCount { get; set; } = 2;
        public int MaxPostImageCount { get; set; } = 4;
        public int MaxImageWidth { get; set; } = 2000;
        public int MaxImageHeight { get; set; } = 2000;
    }
}
