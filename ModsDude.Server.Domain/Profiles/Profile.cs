using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Profiles;
public class Profile(
    RepoId repoId,
    ProfileName name,
    DateTime created)
{
    private readonly HashSet<ModDependency> _modDependencies = [];


    public ProfileId Id { get; init; } = new(Guid.NewGuid());
    public RepoId RepoId { get; } = repoId;

    public IReadOnlySet<ModDependency> ModDependencies => _modDependencies;

    public ProfileName Name { get; set; } = name;
    public DateTime Created { get; } = created;


    public ModDependency AddDependency(ModVersion modVersion, bool lockVersion)
    {
        if (modVersion.Mod.RepoId != RepoId)
        {
            throw new InvalidOperationException($"Cannot add dependency to mod with id '{modVersion.Mod.Id}'. Mod belongs to another repo");
        }

        if (_modDependencies.Any(x => x.ModVersion.Mod == modVersion.Mod))
        {
            throw new InvalidOperationException($"Dependency to mod with id '{modVersion.Mod.Id}' already exists");
        }

        var newDependency = new ModDependency()
        {
            ModVersion = modVersion,
            LockVersion = lockVersion
        };

        _modDependencies.Add(newDependency);
        
        return newDependency;
    }

    public void DeleteDependency(ModDependency dependency)
    {
        if (!_modDependencies.Contains(dependency))
        {
            throw new InvalidOperationException($"Cannot delete dependency on mod '{dependency.ModVersion.Mod.Id}' from profile '{Id}'. Dependency does not belong to profile");
        }

        _modDependencies.Remove(dependency);
    }
}

public readonly record struct ProfileId(Guid Value);
public readonly record struct ProfileName(string Value);
