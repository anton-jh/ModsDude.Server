using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal static class SystemInviteConfigurations
{
    internal static ModelConfigurationBuilder ConfigureSystemInviteConventions(this ModelConfigurationBuilder builder)
    {
        builder
            .HasValueOfConversion<string, SystemInviteId>()
            .HasValueOfConversion<int, SystemInviteUses>();

        return builder;
    }



    internal class SystemInviteEntity : IEntityTypeConfiguration<SystemInvite>
    {
        public void Configure(EntityTypeBuilder<SystemInvite> builder)
        {
            builder.Property(x => x.Expires);
            builder.Property(x => x.UsesLeft);
        }
    }
}
