using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Authorization;
public interface IRepoAuthorizationService
{
    Task<bool> AuthorizeAsync(UserId userId, RepoId repoId, RepoMembershipLevel minimumMembershipLevel, CancellationToken cancellationToken);
}