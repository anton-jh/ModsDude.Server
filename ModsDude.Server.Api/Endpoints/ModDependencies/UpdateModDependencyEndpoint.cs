﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
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

        var modDependency = profile.ModDependencies.FirstOrDefault(x => x.ModVersion.Mod.Id == new ModId(modId));
        if (modDependency is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No dependency on mod '{modId}' found in profile '{profileId}'"));
        }

        if (!modDependency.ModVersion.Mod.Versions.Any(x => x.Id == new ModVersionId(request.VersionId)))
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No version '{request.VersionId}' of mod '{modId}' found in repo '{repoId}'"));
        }

        modDependency.ChangeVersion(new ModVersionId(request.VersionId));
        modDependency.LockVersion = request.LockVersion;

        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ModDependencyDto.FromModel(modDependency));
    }


    public record UpdateModDependencyRequest(string VersionId, bool LockVersion);
}
