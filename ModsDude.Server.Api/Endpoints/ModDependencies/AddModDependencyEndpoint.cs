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

public class AddModDependencyEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies", Add);
    }


    private static async Task<Results<Ok<ModDependencyDto>, BadRequest<CustomProblemDetails>>> Add(
        Guid repoId, Guid profileId, AddModDependencyRequest request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IModRepository modRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .CheckIsAllowedTo(x => x
                .AccessRepoAtLevel(new RepoId(repoId), RepoMembershipLevel.Member))
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        var profile = await profileRepository.GetById(new RepoId(repoId), new ProfileId(profileId), cancellationToken);
        if (profile is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No profile '{profileId}' found in repo '{repoId}'"));
        }

        var modVersion = await modRepository.GetModVersion(new RepoId(repoId), new ModId(request.ModId), new ModVersionId(request.VersionId), cancellationToken);
        if (modVersion is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"No mod '{request.ModId}' found in repo '{repoId}'"));
        }

        if (profile.ModDependencies.Any(x => x.ModVersion.Mod == modVersion.Mod))
        {
            return TypedResults.BadRequest(Problems.ModDependencyExists(profile, modVersion.Mod));
        }

        var modDependency = profile.AddDependency(modVersion, request.LockVersion);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ModDependencyDto.FromModel(modDependency));
    }


    public record AddModDependencyRequest(string ModId, string VersionId, bool LockVersion);
}
