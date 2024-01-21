using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.RepoMemberships;
public interface IRepoMembershipCollection
{
    void Set(RepoId repoId, RepoMembershipLevel level);
    void Remove(RepoId repoId);
}
