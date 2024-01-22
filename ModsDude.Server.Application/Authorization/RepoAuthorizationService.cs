using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Authorization;
public class RepoAuthorizationService(
    IRepoMembershipRepository repoMembershipRepository) : IRepoAuthorizationService
{
    public async Task<bool> AuthorizeAsync(UserId userId, RepoId repoId, RepoMembershipLevel minimumMembershipLevel, CancellationToken cancellationToken)
    {
        var memberships = await repoMembershipRepository.GetByUserIdAsync(userId, cancellationToken);
        var membership = memberships.FirstOrDefault(m => m.RepoId == repoId);

        return membership is not null && membership.Level >= minimumMembershipLevel;
    }
}
