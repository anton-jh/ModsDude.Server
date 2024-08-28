using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class ModVersionEntityTypeConfiguration : IEntityTypeConfiguration<ModVersion>
{
    public void Configure(EntityTypeBuilder<ModVersion> builder)
    {
        builder.Property<RepoId>("RepoId");
        builder.Property<ModId>("ModId");

        builder.HasKey("RepoId", "ModId", nameof(ModVersion.Id));

        builder.HasOne(x => x.Mod).WithMany(x => x.Versions).HasForeignKey("RepoId", "ModId");

        builder.OwnsMany(x => x.Attributes);
    }
}
