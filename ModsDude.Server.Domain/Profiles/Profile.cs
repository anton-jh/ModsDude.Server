using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Profiles;
public class Profile(
    RepoId repoId,
    ProfileName name,
    DateTime created)
{
    public ProfileId Id { get; init; } = new(Guid.NewGuid());
    public RepoId RepoId { get; } = repoId;

    public HashSet<ModDependency> ModDependencies { get; set; } = new();

    public ProfileName Name { get; set; } = name;
    public DateTime Created { get; } = created; // TODO: should probably use DateTimeOffset here right?
}

public readonly record struct ProfileId(Guid Value);
public readonly record struct ProfileName(string Value);
