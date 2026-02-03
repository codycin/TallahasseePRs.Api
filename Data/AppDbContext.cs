using Microsoft.EntityFrameworkCore;
using TallahasseePRs.Api.Models;

namespace TallahasseePRs.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TestEntity> TestEntities => Set<TestEntity>();
}