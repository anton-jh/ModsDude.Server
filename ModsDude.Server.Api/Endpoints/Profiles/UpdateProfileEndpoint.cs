﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class UpdateProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPut("repos/{repoId:guid}/profiles/{profileId:guid}", Update);
    }


    private static async Task<Results<Ok<ProfileDto>, BadRequest<CustomProblemDetails>>> Update(
        Guid repoId, Guid profileId,
        UpdateProfileRequest request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
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
            return TypedResults.BadRequest(Problems.NotFound);
        }

        if (await profileRepository.CheckNameIsTaken(new RepoId(repoId), new ProfileName(request.Name), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }

        profile.Name = new ProfileName(request.Name);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ProfileDto.FromModel(profile));
    }


    public record UpdateProfileRequest(string Name);
}
