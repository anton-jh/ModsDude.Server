using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Repos;
public class Repo(RepoName name, SerializedAdapter adapter, DateTime created)
{
    public RepoId Id { get; private set; } = new(Guid.NewGuid());

    public RepoName Name { get; set; } = name;
    public SerializedAdapter Adapter { get; } = adapter;
    public DateTime Created { get; } = created;
}

public readonly record struct RepoId(Guid Value);
public readonly record struct RepoName(string Value);
public readonly record struct SerializedAdapter(string Value);
