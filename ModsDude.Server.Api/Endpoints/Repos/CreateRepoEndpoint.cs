using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class CreateRepoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos", CreateRepo)
            .RequireAuthorization();
    }


    private static async Task<Results<Ok<RepoDto>, BadRequest<CustomProblemDetails>>> CreateRepo(
        CreateRepoRequest request,
        IRepoRepository repoRepository,
        IUnitOfWork unitOfWork,
        ITimeService timeService,
        ApplicationDbContext dbContext,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var userId = httpContext.User.GetUserId();
        var isTrusted = (await dbContext.Users
            .FirstAsync(x => x.Id == userId, cancellationToken))
            .IsTrusted;
        if (!isTrusted)
        {
            return TypedResults.BadRequest(Problems.NotAuthorized);
        }

        if (await repoRepository.CheckNameIsTaken(new RepoName(request.Name), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }

        var repo = new Repo(new RepoName(request.Name), timeService.Now())
        {
            AdapterData = new AdapterData(
                new AdapterIdentifier(request.AdapterId),
                new AdapterConfiguration(request.AdapterConfiguration))
        };
        repoRepository.AddNewRepo(repo);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(RepoDto.FromModel(repo));
    }


    public record CreateRepoRequest(string Name, string AdapterId, string AdapterConfiguration);
}
