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

public class AddModDependencyEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies", Add);
    }


    private static async Task<Results<Ok<ModDependencyDto>, BadRequest<CustomProblemDetails>>> Add(
        Guid repoId, Guid profileId, AddModDependencyRequest request,
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

        var result = await profileService.AddModDependency(
            new RepoId(repoId),
            new ProfileId(profileId),
            new ModId(request.ModId),
            new ModVersionId(request.VersionId),
            request.LockVersion,
            cancellationToken);

        switch (result)
        {
            case AddModDependencyResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(ModDependencyDto.FromModel(ok.ModDependency));

            case AddModDependencyResult.ProfileNotFound:
                return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No profile '{profileId}' found in repo '{repoId}'"));

            case AddModDependencyResult.ModNotFound:
                return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No mod '{request.ModId}' found in repo '{repoId}'"));
        }
        throw new UnreachableException();
    }


    public record AddModDependencyRequest(string ModId, string VersionId, bool LockVersion);
}
