using TallahasseePRs.Api.DTOs.Posts;

namespace TallahasseePRs.Api.Services
{
    //Defines what posts can do.
    public interface IPostService
    {
        Task<PostResponse> CreateAsync(Guid userId, CreatePostRequest request);
        Task<PostResponse?> GetByIdAsync(Guid postId);
        Task<PostResponse?> UpdateAsync(Guid userId, Guid postId, UpdatePostRequest request);
        Task<bool> DeleteAsync(Guid userId, Guid postId);


    }
}
