﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class DeleteProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{repoId:guid}/profiles/{profileId:guid}", Delete);
    }

    
    private static async Task<Results<Ok, BadRequest<CustomProblemDetails>>> Delete(
        Guid repoId, Guid profileId,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .IsAllowedTo()
            .AccessRepoAtLevel(new RepoId(repoId), RepoMembershipLevel.Member)
            .GetResult()
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        var profile = await profileRepository.GetById(new RepoId(repoId), new ProfileId(profileId), cancellationToken);
        if (profile is null)
        {
            return TypedResults.BadRequest(Problems.NotFound);
        }

        profileRepository.Delete(profile);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
