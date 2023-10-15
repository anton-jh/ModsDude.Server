using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RepoMembershipEntityTypeConfiguration : IEntityTypeConfiguration<RepoMembership>
{
    public void Configure(EntityTypeBuilder<RepoMembership> builder)
    {
        builder.HasKey(x => new { x.UserId, x.RepoId });

        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
        builder.HasOne<Repo>().WithMany().HasForeignKey(x => x.RepoId);

        builder.Property(x => x.Level)
            .HasConversion<RepoMembershipLevelValueConverter>();
    }
}
