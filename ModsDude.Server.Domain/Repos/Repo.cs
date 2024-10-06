namespace ModsDude.Server.Domain.Repos;
public class Repo(
    RepoName name,
    DateTime created)
{
    public RepoId Id { get; init; } = new(Guid.NewGuid());

    public RepoName Name { get; set; } = name;
    public required AdapterData AdapterData { get; init; }
    public DateTime Created { get; } = created;
}

public readonly record struct RepoId(Guid Value);
public readonly record struct RepoName(string Value);

public record AdapterData(AdapterIdentifier Id, AdapterConfiguration Configuration);
public readonly record struct AdapterIdentifier(string Value);
public readonly record struct AdapterConfiguration(string Value);
