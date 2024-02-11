namespace ModsDude.Server.Domain.Repos;
public class Repo(
    RepoName name,
    AdapterScript? modAdapter,
    AdapterScript? savegameAdapter,
    DateTime created)
{
    public RepoId Id { get; init; } = new(Guid.NewGuid());

    public RepoName Name { get; set; } = name;
    public AdapterScript? ModAdapter { get; } = modAdapter;
    public AdapterScript? SavegameAdapter { get; } = savegameAdapter;
    public DateTime Created { get; } = created;
}

public readonly record struct RepoId(Guid Value);
public readonly record struct RepoName(string Value);
public readonly record struct AdapterScript(string Value);
