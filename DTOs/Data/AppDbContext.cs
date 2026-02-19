using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;
using TallahasseePRs.Api.Models;
using TallahasseePRs.Api.Models.Notifications;
using TallahasseePRs.Api.Models.Posts;
using TallahasseePRs.Api.Models.Users;

namespace TallahasseePRs.Api.DTOs.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile>  Profiles => Set<Profile>();
    public DbSet<Lift> Lifts => Set<Lift>();
    public DbSet<PRPost> Posts => Set<PRPost>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // USER
        // Map the C# property `UserName` to the actual DB column name (example: "username")


        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        //Profile
        modelBuilder.Entity<Profile>()
            .HasKey(p => p.UserId);

        modelBuilder.Entity<Profile>()
            .HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<Profile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // PR POSTS
        modelBuilder.Entity<PRPost>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PRPost>()
            .HasOne(p => p.Lift)
            .WithMany()
            .HasForeignKey(p => p.LiftId)
            .OnDelete(DeleteBehavior.Restrict);

        // COMMENTS
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.PRPost)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.PRPostId)
            .OnDelete(DeleteBehavior.Cascade);

        // SELF REFERENCING REPLIES
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);

        // VOTES
        modelBuilder.Entity<Vote>()
            .HasOne(v => v.User)
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vote>()
            .HasOne(v => v.PRPost)
            .WithMany()
            .HasForeignKey(v => v.PRPostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vote>()
            .HasIndex(v => new { v.PRPostId, v.UserId })
            .IsUnique();

        // FOLLOWS
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.FollowerUser)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.FollowedUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowedId)
            .OnDelete(DeleteBehavior.Restrict);

        // REFRESH TOKENS
        modelBuilder.Entity<RefreshToken>()
            .HasOne(r => r.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.TokenHash)
            .IsUnique();
    }


}