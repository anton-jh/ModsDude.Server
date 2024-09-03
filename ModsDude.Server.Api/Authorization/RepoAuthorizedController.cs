using Microsoft.AspNetCore.Mvc;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Api.Authorization;

public class RepoAuthorizedController(
    IRepoAuthorizationService repoAuthorizationService)
    : ControllerBase
{
    protected Task<bool> AuthorizeForRepoAsync(Guid repoId, CancellationToken cancellationToken)
    {
        return repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), new RepoId(repoId), RepoMembershipLevel.Member, cancellationToken);
    }
}
