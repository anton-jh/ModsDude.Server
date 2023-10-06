using MediatR.Pipeline;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Application.Authorization;
internal class RepoAuthorizingPreProcessor : IRequestPreProcessor<IRepoAuthorizedRequest>
{
    private readonly RepoMembershipExtractor _repoMembershipExtractor;


    public RepoAuthorizingPreProcessor(RepoMembershipExtractor repoMembershipExtractor)
    {
        _repoMembershipExtractor = repoMembershipExtractor;
    }


    public async Task Process(IRepoAuthorizedRequest request, CancellationToken cancellationToken)
    {
        RepoId repoId = await request.GetRepoId();
        // TODO: Extract repo memberships from ClaimsPrincipal
        // Maybe create some class for encapsulating a user's memberships with a method to get the level for a specific RepoId
        // Maybe this needs to be in a PipelineBehaviour? Not sure if throwing here will actually stop the pipeline.
    }
}
