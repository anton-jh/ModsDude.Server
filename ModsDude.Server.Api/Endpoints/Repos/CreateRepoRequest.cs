namespace ModsDude.Server.Api.Endpoints.Repos;

public record CreateRepoRequest(
    string Name,
    string SerializedAdapter
);
