using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Application.Services.Extensions;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Api.Endpoints.Repos.MyRepos;

public class MyReposEndpoint(
    ApplicationDbContext dbContext)
    : EndpointBaseAsync
        .WithoutRequest
        .WithResult<IEnumerable<RepoMembershipDto>>
{
    [HttpGet("repos/my-repos")]
    public override async Task<IEnumerable<RepoMembershipDto>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var userId = HttpContext.User.GetUserId();
        var reposQuery = dbContext.RepoMemberships
            .Where(x => x.UserId == userId)
            .Join(dbContext.Repos, mem => mem.RepoId, repo => repo.Id, (mem, repo) => new
            {
                Repo = repo,
                Membership = mem
            })
            .OrderBy(x => x.Repo.Name);
        var repos = await reposQuery.ToListAsync(cancellationToken);
        var dtos = repos.Select(x => new RepoMembershipDto()
        {
            Repo = new RepoDto(x.Repo.Name.Value, x.Repo.Adapter.Value),
            MembershipLevel = RepoMembershipLevelEnumExtensions.Map(x.Membership.Level)
        });

        return dtos;
    }
}
