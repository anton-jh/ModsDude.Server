using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.ModDependencies;

public class UpdateModDependencyEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPut("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies/{modId}", Update);
    }


    private static async Task<Results<Ok<ModDependencyDto>, BadRequest<CustomProblemDetails>>> Update(
        Guid repoId, Guid profileId, string modId, UpdateModDependencyRequest request,
        ClaimsPrincipal claimsPrincipal,
        IRepoAuthorizationService repoAuthorizationService,
        IProfileService profileService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(claimsPrincipal.GetUserId(), new(repoId), RepoMembershipLevel.Member, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Member));
        }

        var result = await profileService.UpdateModDependency(
            new RepoId(repoId),
            new ProfileId(profileId),
            new ModId(modId),
            new ModVersionId(request.VersionId),
            request.LockVersion,
            cancellationToken);

        switch (result)
        {
            case UpdateModDependencyResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(ModDependencyDto.FromModel(ok.ModDependency));

            case UpdateModDependencyResult.DependencyNotFound:
                return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No dependency on mod '{modId}' found in profile '{profileId}'"));

            case UpdateModDependencyResult.ModVersionNotFound:
                return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No version '{request.VersionId}' of mod '{modId}' found in repo '{repoId}'"));

            case UpdateModDependencyResult.ProfileNotFound:
                return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No profile '{profileId}' found in repo '{repoId}'"));
        }
        throw new UnreachableException();
    }


    public record UpdateModDependencyRequest(string VersionId, bool LockVersion);
}
