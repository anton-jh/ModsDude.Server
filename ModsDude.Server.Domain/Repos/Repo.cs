using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Repos;
public class Repo(RepoName name)
{
    public RepoId Id { get; private set; } = RepoId.NewId();

    public RepoName Name { get; } = name;
}

public class RepoId : GuidId<RepoId>;
