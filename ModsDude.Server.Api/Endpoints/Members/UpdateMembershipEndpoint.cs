﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Members;

public class UpdateMembershipEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPut("repos/{repoId:guid}/members/{userId}", UpdateMembership);
    }


    private async Task<Results<Ok, BadRequest<CustomProblemDetails>>> UpdateMembership(
        Guid repoId, string userId,
        UpdateMembershipRequest request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IRepoRepository repoRepository,
        IRepoMembershipRepository repoMembershipRepository,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var repo = await repoRepository.GetById(new RepoId(repoId));
        if (repo is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"Repo '{repoId}' does not exist"));
        }

        var subjectMembership = repo.GetMembership(new UserId(userId));
        if (subjectMembership is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"Member '{userId}' not found"));
        }

        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .CheckIsAllowedTo(x => x
                .ChangeOthersMembership(subjectMembership)
                .GrantAccessToRepo(new RepoId(repoId), request.NewLevel))
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        subjectMembership.Level = request.NewLevel;
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }


    public record UpdateMembershipRequest(RepoMembershipLevel NewLevel);
}
