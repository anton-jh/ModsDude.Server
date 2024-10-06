using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.ModDependencies;

public class DeleteModDependencyEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies/{modId}", Delete);
    }


    private static async Task<Results<Ok, BadRequest<CustomProblemDetails>>> Delete(
        Guid repoId, Guid profileId, string modId,
        ClaimsPrincipal claimsPrincipal,
        IRepoAuthorizationService repoAuthorizationService,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(claimsPrincipal.GetUserId(), new(repoId), RepoMembershipLevel.Member, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Member));
        }

        var profile = await profileRepository.GetById(new RepoId(repoId), new ProfileId(profileId), cancellationToken);
        if (profile is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No profile '{profileId}' found in repo '{repoId}'"));
        }

        if (!profile.HasDependencyOn(new ModId(modId)))
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No dependency on mod '{modId}' found in profile '{profileId}'"));
        }

        profile.DeleteDependency(new ModId(modId));
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
