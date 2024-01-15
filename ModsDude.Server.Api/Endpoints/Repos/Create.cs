using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModsDude.Server.Api.Authorization;

namespace ModsDude.Server.Api.Endpoints.Repos;

public class Create
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult
{
    [HttpPost("repos/create")]
    [Authorize(Scopes.Repo.Create)]
    public override Task<ActionResult> HandleAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult((ActionResult)Ok());
    }
}
