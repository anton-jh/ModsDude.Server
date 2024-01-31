using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RepoEntityTypeConfiguration : IEntityTypeConfiguration<Repo>
{
    public void Configure(EntityTypeBuilder<Repo> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name);
        builder.Property(x => x.Adapter);
        builder.Property(x => x.Created);
    }
}
