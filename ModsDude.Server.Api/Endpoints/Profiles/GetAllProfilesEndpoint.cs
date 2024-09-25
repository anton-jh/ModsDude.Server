using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class GetAllProfilesEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("repos/{repoId:guid}/profiles", GetAll);
    }


    private static async Task<Results<Ok<IEnumerable<ProfileDto>>, BadRequest<CustomProblemDetails>>> GetAll(
        Guid repoId,
        ApplicationDbContext dbContext,
        IRepoAuthorizationService repoAuthorizationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), new(repoId), RepoMembershipLevel.Guest, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Guest));
        }

        var profiles = await dbContext.Profiles
            .Where(x => x.RepoId == new RepoId(repoId))
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(ProfileDto.FromModel);

        return TypedResults.Ok(dtos);
    }
}
