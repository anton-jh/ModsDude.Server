using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class UpdateRepoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPut("repos/{id:guid}", UpdateRepo);
    }


    private static async Task<Results<Ok<RepoDto>, BadRequest<CustomProblemDetails>>> UpdateRepo(
        Guid id,
        UpdateRepoRequest request,
        HttpContext httpContext,
        IUnitOfWork unitOfWork,
        IRepoService repoService,
        IRepoAuthorizationService repoAuthorizationService,
        CancellationToken cancellationToken)
    {
        var repoId = new RepoId(id);

        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), repoId, RepoMembershipLevel.Admin, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Admin));
        }
        var name = new RepoName(request.Name);

        var result = await repoService.Update(repoId, name, cancellationToken);

        switch (result)
        {
            case UpdateRepoResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(RepoDto.FromModel(ok.Repo));

            case UpdateRepoResult.NotFound:
                return TypedResults.BadRequest(Problems.NotFound);

            case UpdateRepoResult.NameTaken:
                return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }
        throw new UnreachableException();
    }


    public record UpdateRepoRequest(string Name);
}
