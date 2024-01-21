using Ardalis.ApiEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class CreateRepoEndpoint(
    ISender sender,
    IUnitOfWork unitOfWork)
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

        switch (result)
        {
            case CreateRepoResult.Ok ok:
                await unitOfWork.CommitAsync();
                return Ok(new RepoDto(ok.Repo.Name.Value, ok.Repo.Adapter.Value));

            case CreateRepoResult.NameTaken:
                return Conflict();

            default:
                throw new UnreachableException();
        }
    }
}
