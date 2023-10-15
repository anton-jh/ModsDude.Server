using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.ValueConverters;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal static class RepoConfigurations
{
    public static ModelConfigurationBuilder ConfigureRepoConventions(this ModelConfigurationBuilder builder)
    {
        builder
            .HasGuidIdConversion<RepoId>()
            .HasValueOfConversion<string, RepoName>();

        return builder;
    }


    public class RepoEntity : IEntityTypeConfiguration<Repo>
    {
        public void Configure(EntityTypeBuilder<Repo> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name);
        }
    }
}
