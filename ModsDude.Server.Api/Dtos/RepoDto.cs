using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Api.Dtos;

public record RepoDto(Guid Id, string Name, string? ModAdapter, string? SavegameAdapter)
{
    public static RepoDto FromModel(Repo repo)
    {
        return new(
            repo.Id.Value,
            repo.Name.Value,
            repo.ModAdapter?.Value,
            repo.SavegameAdapter?.Value);
    }
}
