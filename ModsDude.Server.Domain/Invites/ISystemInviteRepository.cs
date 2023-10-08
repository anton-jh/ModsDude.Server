namespace ModsDude.Server.Domain.Invites;
public interface ISystemInviteRepository
{
    Task<SystemInvite?> GetByIdAsync(SystemInviteId id, CancellationToken cancellationToken = default);
    void Delete(SystemInvite systemInvite);
}
