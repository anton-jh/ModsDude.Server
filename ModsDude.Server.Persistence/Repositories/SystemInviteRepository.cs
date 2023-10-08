using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class SystemInviteRepository : ISystemInviteRepository
{
    private readonly ApplicationDbContext _dbContext;


    public SystemInviteRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public void Delete(SystemInvite systemInvite)
    {
        _dbContext.SystemInvites
            .Remove(systemInvite);
    }

    public async Task<SystemInvite?> GetByIdAsync(SystemInviteId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.SystemInvites
            .FindAsync(new object[] { id }, cancellationToken);
    }
}
