namespace TallahasseePRs.Api.Models;

public class User
{
    public int Id { get; set; }   // <-- primary key
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Member";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}