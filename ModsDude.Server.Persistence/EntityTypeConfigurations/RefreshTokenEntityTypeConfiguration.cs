using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.Extensions;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RefreshTokenEntityTypeConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasValueOfConversion<string, RefreshTokenId>();

        builder.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);

        builder.Property(x => x.FamilyId)
            .HasGuidIdConversion();
        builder.HasIndex(x => x.FamilyId);

        builder.Property(x => x.Created);

        builder.Property(x => x.Expires);
    }
}
