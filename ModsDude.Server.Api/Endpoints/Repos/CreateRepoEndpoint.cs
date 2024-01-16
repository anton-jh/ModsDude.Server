using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class CreateRepoEndpoint(
    ISender sender)
    : EndpointBaseAsync
        .WithRequest<CreateRepoRequest>
        .WithActionResult<RepoDto>
{
    [HttpPost("repos/create")]
    [Authorize(Scopes.Repo.Create)]
    public override async Task<ActionResult<RepoDto>> HandleAsync(CreateRepoRequest request, CancellationToken cancellationToken = default)
    {
        var name = RepoName.From(request.Name);
        var serializedAdapter = SerializedAdapter.From(request.SerializedAdapter);

        var result = await sender.Send(new CreateRepoCommand(name, serializedAdapter), cancellationToken);

        return result switch
        {
            CreateRepoResult.Ok ok => Ok(new RepoDto(ok.Repo.Name.Value, ok.Repo.Adapter.Value)),
            CreateRepoResult.NameTaken => Conflict(),
            _ => throw new UnreachableException()
        };
    }
}
