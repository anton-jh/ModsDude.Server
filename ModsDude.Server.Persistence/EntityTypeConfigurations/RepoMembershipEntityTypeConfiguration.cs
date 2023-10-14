using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModsDude.Server.Persistence.EntityTypeConfigurations;
internal class RepoMembershipEntityTypeConfiguration : IEntityTypeConfiguration<RepoMembership>
{
    public void Configure(EntityTypeBuilder<RepoMembership> builder)
    {
        builder.HasKey(m => new { m.UserId, m.RepoId });

        builder.HasOne<User>().WithMany().HasForeignKey(m => m.UserId);
        builder.HasOne<Repo>().WithMany().HasForeignKey(m => m.RepoId);
    }
}
