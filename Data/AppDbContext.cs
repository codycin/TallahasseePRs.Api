using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Models;

namespace TallahasseePRs.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Profile>  Profiles => Set<Profile>();
    public DbSet<Lift> Lifts => Set<Lift>();
    public DbSet<PRPost> Posts => Set<PRPost>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>() //Ensure no duplicates based off of emails
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<Profile>() //Sets unqiue identitfier 
            .HasKey(p => p.UserId);
        modelBuilder.Entity<Profile>() //Links Profile to User with profile foreign key User ID to the User Primary
            .HasOne(p=>p.User)
            .WithOne()
            .HasForeignKey<Profile>(p=>p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PRPost>() //relation between a pr post and one user. User can have many PR Posts
            .HasOne(p=>p.User)
            .WithMany()
            .HasForeignKey(p=>p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PRPost>() //relation between a pr post and lifts. Lifts can be in many other PR posts
            .HasOne(p=>p.Lift)
            .WithMany()
            .HasForeignKey(p=>p.LiftId)
            .OnDelete(DeleteBehavior.Cascade);


    }
}