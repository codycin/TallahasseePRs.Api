namespace TallahasseePRs.Api.DTOs.Auth
{
    public class LoginRequest
    {
        public string EmailOrUserName { get; set; } = "";
        public string Password { get; set; } = "";

    }
}
