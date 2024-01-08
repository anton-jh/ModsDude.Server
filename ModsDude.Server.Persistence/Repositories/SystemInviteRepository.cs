using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class SystemInviteRepository(ApplicationDbContext dbContext) : ISystemInviteRepository
{
    public void Delete(SystemInvite systemInvite)
    {
        dbContext.SystemInvites
            .Remove(systemInvite);
    }

    public async Task<SystemInvite?> GetByIdAsync(SystemInviteId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.SystemInvites
            .FindAsync(new object[] { id }, cancellationToken);
    }
}
