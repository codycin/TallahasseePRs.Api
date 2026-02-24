using System.ComponentModel.DataAnnotations;

namespace TallahasseePRs.Api.DTOs.Comments
{
    public sealed record AddCommentRequest(
    [Required, MinLength(1), MaxLength(2000)]
    string Body
);

}
