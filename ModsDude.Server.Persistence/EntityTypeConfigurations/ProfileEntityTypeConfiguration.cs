using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class ProfileEntityTypeConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne<Repo>().WithMany().HasForeignKey(x => x.RepoId);

        builder.Property(x => x.Name);
        builder.Property(x => x.Created);

        builder.HasIndex(x => new { x.RepoId, x.Name }).IsUnique();
    }
}
