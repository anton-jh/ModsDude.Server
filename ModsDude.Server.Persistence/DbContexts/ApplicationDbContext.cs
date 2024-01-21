using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Persistence.DbContexts;
public class ApplicationDbContext(DbContextOptions options) : DbContext(options), IUnitOfWork
{
    public required DbSet<User> Users { get; init; }
    public required DbSet<Repo> Repos { get; init; }
    public required DbSet<RepoMembership> RepoMemberships { get; init; }


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }


    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }
}
