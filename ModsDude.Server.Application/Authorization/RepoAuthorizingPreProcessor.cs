using MediatR.Pipeline;
using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Application.Services.Extensions;
using ModsDude.Server.Domain.RepoMemberships;

namespace ModsDude.Server.Application.Authorization;
public class RepoAuthorizingPreProcessor(IRepoMembershipRepository repoMembershipRepository) : IRequestPreProcessor<IRepoAuthorizedRequest>
{
    public async Task Process(IRepoAuthorizedRequest request, CancellationToken cancellationToken)
    {
        var repoId = await request.GetRepoId();
        var userId = request.ClaimsPrincipal.GetUserId();
        var memberships = await repoMembershipRepository.GetByUserIdAsync(userId, cancellationToken);
        var membership = memberships.FirstOrDefault(m => m.RepoId == repoId);

        if (membership is null || membership.Level < request.MinimumMembershipLevel)
        {
            throw new RepoAccessDeniedException();
        }
    }
}
