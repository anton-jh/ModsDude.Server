using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Domain.Repos;
public class Repo(
    RepoName name,
    DateTime created)
{
    private readonly HashSet<RepoMembership> _memberships = [];


    public RepoId Id { get; init; } = new(Guid.NewGuid());

    public RepoName Name { get; set; } = name;
    public required AdapterData AdapterData { get; init; }
    public DateTime Created { get; } = created;

    
    public void SetMembershipLevel(UserId userId, RepoMembershipLevel level)
    {
        if (_memberships.FirstOrDefault(x => x.UserId == userId) is RepoMembership existing)
        {
            existing.Level = level;
        }
        else
        {
            var membership = new RepoMembership(
                userId, Id, level);
            _memberships.Add(membership);
        }
    }

    public void KickMember(UserId userId)
    {
        var membership = _memberships.FirstOrDefault(x => x.UserId == userId)
            ?? throw new InvalidOperationException($"User '{userId}' is not a member of repo '{Id}'");

        _memberships.Remove(membership);
    }

    public bool HasMember(UserId userId)
    {
        return _memberships.Any(x => x.UserId == userId);
    }

    public RepoMembership? GetMembership(UserId userId)
    {
        return _memberships.FirstOrDefault(x => x.UserId == userId);
    }
}

public readonly record struct RepoId(Guid Value);
public readonly record struct RepoName(string Value);

public record AdapterData(AdapterIdentifier Id, AdapterConfiguration Configuration);
public readonly record struct AdapterIdentifier(string Value);
public readonly record struct AdapterConfiguration(string Value);
