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
            builder.HasIndex(user => user.Username).IsUnique();
        }
    }
}

