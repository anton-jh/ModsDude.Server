using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class DeleteRepoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{id:guid}", DeleteRepo);
    }


    private static async Task<Results<Ok, BadRequest<CustomProblemDetails>>> DeleteRepo(
        Guid id,
        IUnitOfWork unitOfWork,
        IRepoRepository repoRepository,
        IRepoAuthorizationService repoAuthorizationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), new RepoId(id), RepoMembershipLevel.Admin, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Admin));
        }

        var repo = await repoRepository.GetById(new RepoId(id));
        if (repo is null)
        {
            return TypedResults.BadRequest(Problems.NotFound);
        }

        repoRepository.Delete(repo);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
