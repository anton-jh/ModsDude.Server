using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

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
        IRepoRepository repoRepository,
        IRepoAuthorizationService repoAuthorizationService,
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

        if (await repoRepository.CheckNameIsTaken(new RepoName(request.Name), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }

        repo.Name = new RepoName(request.Name);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(RepoDto.FromModel(repo));
    }


    public record UpdateRepoRequest(string Name);
}
