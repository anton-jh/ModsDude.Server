using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Mods;

public class Mod
{
    private readonly List<ModVersion> _versions = [];


    public required RepoId RepoId { get; init; }
    public required ModId Id { get; init; }

    public IReadOnlyList<ModVersion> Versions => _versions;

    public required DateTimeOffset Created { get; init; }


    public ModVersion AddVersion(
        ModVersionId id,
        IReadOnlySet<ModAttribute> attributes,
        DateTimeOffset created,
        string description,
        string displayName)
    {
        var newVersion = new ModVersion()
        {
            Id = id,
            Attributes = new(attributes),
            Created = created,
            Description = description,
            DisplayName = displayName,
            Mod = this
        };

        _versions.Add(newVersion);

        return newVersion;
    }

    public void RemoveVersion(ModVersionId id)
    {
        _versions.Remove(_versions.First(x => x.Id == id));
    }
}


public readonly record struct ModId(string Value);
