using Microsoft.AspNetCore.Mvc;

namespace ModsDude.Server.Api.Endpoints.Repos;

public record UpdateRepoRequest(
    string Name
);
