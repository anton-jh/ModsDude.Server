using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.ModDependencies;

public class GetAllModDependenciesEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies", GetAll);
    }


    private static async Task<Results<Ok<IEnumerable<ModDependencyDto>>, BadRequest<CustomProblemDetails>>> GetAll(
        Guid repoId, Guid profileId,
        ClaimsPrincipal claimsPrincipal,
        IRepoAuthorizationService repoAuthorizationService,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(claimsPrincipal.GetUserId(), new(repoId), RepoMembershipLevel.Guest, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Guest));
        }

        var modDependencies = await dbContext.Profiles
            .Where(x => x.RepoId == new RepoId(repoId) && x.Id == new ProfileId(profileId))
            .SelectMany(x => x.ModDependencies)
            .ToListAsync(cancellationToken);

        var dtos = modDependencies.Select(ModDependencyDto.FromModel);

        return TypedResults.Ok(dtos);
    }
}
