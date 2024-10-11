﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class DeleteRepoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{repoId:guid}", DeleteRepo);
    }


    private static async Task<Results<Ok, BadRequest<CustomProblemDetails>>> DeleteRepo(
        Guid repoId,
        ClaimsPrincipal claimsPrincipal,
        IUnitOfWork unitOfWork,
        IRepoRepository repoRepository,
        IUserRepository userRepository,
        CancellationToken cancellationToken)
    {
        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .IsAllowedTo()
            .AccessRepoAtLevel(new RepoId(repoId), RepoMembershipLevel.Admin)
            .GetResult()
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        var repo = await repoRepository.GetById(new RepoId(repoId));
        if (repo is null)
        {
            return TypedResults.BadRequest(Problems.NotFound);
        }

        repoRepository.Delete(repo);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
