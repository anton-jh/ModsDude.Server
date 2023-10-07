using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;

internal static class RepoMembershipConfigurations
{
    internal static ModelConfigurationBuilder ConfigureRepoMembershipConventions(this ModelConfigurationBuilder builder)
    {
        builder
            .Properties<RepoMembershipLevel>()
            .HaveConversion<RepoMembershipLevelValueConverter>();

        return builder;
    }


    internal class RepoMembershipEntity : IEntityTypeConfiguration<RepoMembership>
    {
        public void Configure(EntityTypeBuilder<RepoMembership> builder)
        {
            builder.HasKey(m => new { m.UserId, m.RepoId });

            builder.HasOne<User>().WithMany().HasForeignKey(m => m.UserId);
            builder.HasOne<Repo>().WithMany().HasForeignKey(m => m.RepoId);
        }
    }
}

