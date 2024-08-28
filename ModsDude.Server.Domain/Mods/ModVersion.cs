using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Mods;

public class ModVersion
{
    public required ModVersionId Id { get; init; }
    public required ModId ModId { get; init; }
    public required RepoId RepoId { get; init; }

    public required string DisplayName { get; set; }
    public required string Description { get; set; }

    public HashSet<ModTag> Tags { get; private set; } = new();
    public HashSet<ModProperty> Properties { get; private set; } = new();

    public required DateTimeOffset Created { get; init; }
}


public readonly record struct ModVersionId(string Value);
