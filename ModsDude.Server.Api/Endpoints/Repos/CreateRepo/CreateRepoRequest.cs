namespace ModsDude.Server.Api.Endpoints.Repos.Create;

public record CreateRepoRequest(
    string Name,
    string SerializedAdapter
);
