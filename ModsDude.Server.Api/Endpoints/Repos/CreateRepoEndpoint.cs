using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class CreateRepoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos", CreateRepo)
            .RequireAuthorization(Scopes.Repo.Create);
    }


    private static async Task<Results<Ok<RepoDto>, BadRequest<CustomProblemDetails>>> CreateRepo(
        CreateRepoRequest request,
        IRepoService repoService,
        IUnitOfWork unitOfWork,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.ModAdapterScript is null && request.SavegameAdapterScript is null)
        {
            //return BadRequest("Cannot create repo with both mod- and savegame-adapters null"); TODO
        }

        var userId = httpContext.User.GetUserId();
        var name = new RepoName(request.Name);
        AdapterScript? modAdapter = request.ModAdapterScript is null
            ? null
            : new AdapterScript(request.ModAdapterScript);
        AdapterScript? savegameAdapter = request.SavegameAdapterScript is null
            ? null
            : new AdapterScript(request.SavegameAdapterScript);

        var result = await repoService.Create(name, modAdapter, savegameAdapter, userId, cancellationToken);

        switch (result)
        {
            case CreateRepoResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(RepoDto.FromModel(ok.Repo));
                
            case CreateRepoResult.NameTaken:
                return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }
        throw new UnreachableException();
    }


    public record CreateRepoRequest(string Name, string? ModAdapterScript, string? SavegameAdapterScript);
}
