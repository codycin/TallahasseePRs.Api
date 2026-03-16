namespace TallahasseePRs.Api.Data.Configurations
{
    public class R2Options
    {
        public string AccountId { get; set; } = string.Empty;
        public string AccessKeyId {  get; set; } = string.Empty;

        public string SecretAccessKey { get; set; } = "";
        public string BucketName { get; set; } = "";

        // For public file delivery custom domain
        public string PublicBaseUrl { get; set; } = "";

        // Optional: if you want to override directly
        public string Endpoint => $"https://{AccountId}.r2.cloudflarestorage.com";
    }
}
