using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

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
        IRepoService repoService,
        IRepoAuthorizationService repoAuthorizationService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var repoId = new RepoId(id);

        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), repoId, RepoMembershipLevel.Admin, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Admin));
        }

        var result = await repoService.Delete(repoId, cancellationToken);

        switch (result)
        {
            case DeleteRepoResult.Ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok();

            case DeleteRepoResult.NotFound:
                return TypedResults.BadRequest(Problems.NotFound);
        }
        throw new UnreachableException();
    }
}
