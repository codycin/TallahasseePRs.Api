using TallahasseePRs.Api.Models.Enums;

namespace TallahasseePRs.Api.DTOs.Judging
{
    public sealed class JudgeRequest
    {
        public PRstatus Status;
        public String JudgeNote { get; set; } = "";
    }
}
