using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Domain.RepoMemberships;
public interface IRepoMembershipRepository
{
    Task<IEnumerable<RepoMembership>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RepoMembership>> GetByRepoIdAsync(RepoId repoId, CancellationToken cancellationToken = default);
}
