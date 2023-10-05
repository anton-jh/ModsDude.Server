using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Domain.RepoMemberships;
public class RepoMembership
{
    public RepoMembership(UserId userId, RepoId repoId, RepoMembershipLevel level)
    {
        UserId = userId;
        RepoId = repoId;
        Level = level;
    }


    public UserId UserId { get; }
    public RepoId RepoId { get; }
    public RepoMembershipLevel Level { get; }
}
