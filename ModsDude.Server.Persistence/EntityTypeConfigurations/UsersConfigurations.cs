using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;

internal static class UsersConfigurations
{
    internal static ModelConfigurationBuilder ConfigureUserConventions(this ModelConfigurationBuilder builder)
    {
        builder
            .HasGuidIdConversion<UserId>()
            .HasValueOfConversion<string, RefreshTokenId>()
            .HasGuidIdConversion<RefreshTokenFamilyId>()
            .HasValueOfConversion<string, Username>()
            .HasValueOfConversion<string, PasswordHash>();

        return builder;
    }


    internal class UserEntity : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.Username).IsUnique();
        }
    }

    internal class RefreshTokenEntity : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

            builder.Property(x => x.FamilyId);
            builder.Property(x => x.Created);
            builder.Property(x => x.Expires);

            builder.HasIndex(x => x.FamilyId);
        }
    }
}
