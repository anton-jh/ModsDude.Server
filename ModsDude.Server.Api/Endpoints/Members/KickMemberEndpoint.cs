using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Members;

public class KickMemberEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{repoId:guid}/members/{userId}", KickMember);
    }


    private async Task<Results<Ok, BadRequest<CustomProblemDetails>>> KickMember(
        Guid repoId, string userId,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken,
        IRepoRepository repoRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
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
                .ChangeOthersMembership(subjectMembership))
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        if (repo.IsOnlyAdmin(new UserId(userId)))
        {
            return TypedResults.BadRequest(Problems.CannotKickOnlyAdmin);
        }

        repo.KickMember(new UserId(userId));
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
