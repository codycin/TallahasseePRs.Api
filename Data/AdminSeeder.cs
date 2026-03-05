using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Data;
using TallahasseePRs.Api.Models.Users;

public static class AdminSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // if an admin already exists do nothing
        if (await context.Users.AnyAsync(u => u.Role == "Admin"))
            return;

        var email = "admin";
        var password = "admin"; // change after first login

        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            Role = "Admin"
        };

        admin.PasswordHash = hasher.HashPassword(admin, password);

        var profile = new Profile
        {
            UserId = admin.Id
        };

        context.Users.Add(admin);
        context.Profiles.Add(profile);

        await context.SaveChangesAsync();
    }
}