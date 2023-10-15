using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Persistence.Extensions;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class SystemInviteEntityTypeConfiguration : IEntityTypeConfiguration<SystemInvite>
{
    public void Configure(EntityTypeBuilder<SystemInvite> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueOfConversion<string, SystemInviteId>();

        builder.Property(x => x.Expires);
        builder.Property(x => x.UsesLeft)
            .HasValueOfConversion<int, SystemInviteUses>();
    }
}