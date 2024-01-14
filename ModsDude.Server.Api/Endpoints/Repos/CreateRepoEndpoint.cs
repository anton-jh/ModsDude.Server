using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class CreateRepoEndpoint
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult
{
    [HttpPost("repos/create")]
    public override Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((ActionResult)Ok());
    }
}
