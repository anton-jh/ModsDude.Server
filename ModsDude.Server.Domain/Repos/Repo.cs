using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Repos;
public class Repo(RepoName name, SerializedAdapter adapter, DateTime created)
{
    public RepoId Id { get; private set; } = RepoId.NewId();

    public RepoName Name { get; } = name;
    public SerializedAdapter Adapter { get; } = adapter;
    public DateTime Created { get; } = created;
}

public class RepoId : GuidId<RepoId>;
