using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Api.Endpoints.Repos;

public record RepoDto(
    Guid Id,
    string Name,
    string SerializedAdapter
)
{
    public static RepoDto FromRepo(Repo repo)
    {
        return new(repo.Id.Value, repo.Name.Value, repo.Adapter.Value);
    }
}
