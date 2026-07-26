using TallahasseePRs.Api.DTOs.Media;

namespace TallahasseePRs.Api.Services.Media
{
    public interface IMediaResponseMapper
    {
        MediaResponse? ToResponse(Models.Media? media);
    }
}
