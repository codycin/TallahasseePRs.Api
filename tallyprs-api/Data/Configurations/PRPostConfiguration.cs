using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TallahasseePRs.Api.Models.Posts;

namespace TallahasseePRs.Api.Data.Configurations
{
    public class PRPostConfiguration : IEntityTypeConfiguration<PRPost>
    {
        public void Configure(EntityTypeBuilder<PRPost> b)
        {
            b.HasIndex(p => new { p.CreatedAt, p.Id });
            b.HasIndex(p => new { p.UserId, p.CreatedAt, p.Id });
        }
    }
}
