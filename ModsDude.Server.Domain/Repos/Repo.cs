using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Repos;
public class Repo
{
    public Repo(RepoName name)
    {
        Name = name;

        Id = RepoId.NewId();
    }


    public RepoId Id { get; private set; }

    public RepoName Name { get; }
}

public class RepoId : GuidId<RepoId>;
