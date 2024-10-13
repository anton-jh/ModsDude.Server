﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class CreateProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos/{repoId:guid}/profiles", Create);
    }


    private static async Task<Results<Ok<ProfileDto>, BadRequest<CustomProblemDetails>>> Create(
        Guid repoId,
        CreateProfileRequest request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IProfileRepository profileRepository,
        ITimeService timeService,
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

        if (await profileRepository.CheckNameIsTaken(new RepoId(repoId), new ProfileName(request.Name), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }

        var profile = new Profile(new RepoId(repoId), new ProfileName(request.Name), timeService.Now());
        profileRepository.AddNewProfile(profile);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ProfileDto.FromModel(profile));
    }


    public record CreateProfileRequest(string Name);
}
