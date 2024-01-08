using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.Extensions;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueOfConversion<string, UserId>();

        builder.Property(x => x.Username)
            .HasValueOfConversion<string, Username>();
        builder.HasIndex(x => x.Username).IsUnique();
    }
}
