using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.EntityTypeConfigurations;

namespace ModsDude.Server.Persistence.DbContexts;
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public required DbSet<User> Users { get; init; }
    public required DbSet<Repo> Repos { get; init; }
    public required DbSet<RepoMembership> RepoMemberships { get; init; }
    public required DbSet<RefreshToken> RefreshTokens { get; init; }
    public required DbSet<SystemInvite> SystemInvites { get; init; }


    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .ConfigureSystemInviteConventions()
            .ConfigureRepoMembershipConventions()
            .ConfigureRepoConventions()
            .ConfigureUserConventions();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }


    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return SaveChangesAsync(cancellationToken);
    }
}
