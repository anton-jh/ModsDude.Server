using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new(x));

        builder.Property(x => x.Username)
            .HasConversion(x => x.Value, x => new(x));
        builder.HasIndex(x => x.Username).IsUnique();

        builder.HasMany(x => x.RepoMemberships).WithOne().HasForeignKey(x => x.UserId);
    }
}
