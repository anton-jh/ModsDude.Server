using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class RepoMembershipRepository : IRepoMembershipRepository
{
    private readonly ApplicationDbContext _dbContext;


    public RepoMembershipRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<IEnumerable<RepoMembership>> GetByRepoIdAsync(RepoId repoId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RepoMemberships
            .Where(m => m.RepoId == repoId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<RepoMembership>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RepoMemberships
            .Where(m => m.UserId == userId)
            .ToListAsync();
    }
}
