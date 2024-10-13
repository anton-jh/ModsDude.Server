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
        builder.MapDelete("repos/{repoId:guid}/members/{userIdm}", KickMember);
    }


    private async Task<Results<Ok, BadRequest<CustomProblemDetails>>> KickMember(
        Guid repoId, string userId,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        var subjectUser = await userRepository.GetByIdAsync(new UserId(userId), cancellationToken);
        var subjectMembership = subjectUser?.RepoMemberships.FirstOrDefault(x => x.RepoId == new RepoId(repoId));
        if (subjectUser is null || subjectMembership is null)
        {
            return TypedResults.BadRequest(Problems.NotFound.With(x => x.Detail = $"Member {userId} does not exist"));
        }

        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .IsAllowedTo()
            .ChangeOthersMembership(subjectMembership)
            .GetResult()
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        subjectUser.LeaveRepo(new RepoId(repoId));
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
