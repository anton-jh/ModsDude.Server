using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RepoEntityTypeConfiguration : IEntityTypeConfiguration<Repo>
{
    public void Configure(EntityTypeBuilder<Repo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new RepoId(x));

        builder.Property(x => x.Name)
            .HasConversion(x => x.Value, x => new(x));
        builder.Property(x => x.Adapter)
            .HasConversion(x => x.Value, x => new(x));
        builder.Property(x => x.Created);
    }
}
