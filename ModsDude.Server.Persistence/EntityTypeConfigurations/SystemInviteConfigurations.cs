using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal static class SystemInviteConfigurations
{
    public static ModelConfigurationBuilder ConfigureSystemInviteConventions(this ModelConfigurationBuilder builder)
    {
        builder
            .HasValueOfConversion<string, SystemInviteId>()
            .HasValueOfConversion<int, SystemInviteUses>();

        return builder;
    }
}
