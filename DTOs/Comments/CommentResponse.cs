namespace TallahasseePRs.Api.DTOs.Comments
{
    public sealed record CommentResponse(
    Guid Id,
    Guid PostId,
    Guid UserId,
    string Body,
    DateTime CreatedAt,
    Guid? ParentCommentId,
    List<CommentResponse> Replies
);
}
