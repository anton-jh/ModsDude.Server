using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.Extensions;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RepoEntityTypeConfiguration : IEntityTypeConfiguration<Repo>
{
    public void Configure(EntityTypeBuilder<Repo> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasGuidIdConversion();

        builder.Property(x => x.Name)
            .HasValueOfConversion<string, RepoName>();
        builder.Property(x => x.Adapter)
            .HasValueOfConversion<string, SerializedAdapter>();
        builder.Property(x => x.Created);
    }
}
