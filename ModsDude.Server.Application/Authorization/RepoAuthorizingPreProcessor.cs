using MediatR.Pipeline;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Application.Authorization;
internal class RepoAuthorizingPreProcessor : IRequestPreProcessor<IRepoAuthorizedRequest>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly RepoMembershipExtractor _repoMembershipExtractor;


    public RepoAuthorizingPreProcessor(ApplicationDbContext dbContext, RepoMembershipExtractor repoMembershipExtractor)
    {
        _dbContext = dbContext;
        _repoMembershipExtractor = repoMembershipExtractor;
    }


    public async Task Process(IRepoAuthorizedRequest request, CancellationToken cancellationToken)
    {
        RepoId repoId = await request.GetRepoId();
        // TODO: Extract repo memberships from ClaimsPrincipal
        // Maybe create some class for encapsulating a user's memberships with a method to get the level for a specific RepoId
    }
}
