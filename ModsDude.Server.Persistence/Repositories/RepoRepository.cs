using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class RepoRepository(ApplicationDbContext dbContext) : IRepoRepository
{
    public Task<bool> CheckNameIsTaken(RepoName name, CancellationToken cancellationToken)
    {
        return dbContext.Repos.AnyAsync(x => x.Name == name, cancellationToken);
    }

    public void SaveNewRepo(Repo repo)
    {
        dbContext.Repos.Add(repo);
    }
}
