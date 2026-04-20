namespace TallahasseePRs.Api.Services.Media
{
    public interface IVideoProcessingService
    {
        Task ProcessAsync(Guid mediaId, CancellationToken cancellationToken = default);
    }
}
