using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TallahasseePRs.Api.Models.Users;

public sealed class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> b)
    {
        b.HasKey(x => new { x.FollowerId, x.FollowedId }); // composite PK to prevent dups
        b.HasIndex(x => x.FollowerId);
        b.HasIndex(x => x.FollowedId); 
    }
}