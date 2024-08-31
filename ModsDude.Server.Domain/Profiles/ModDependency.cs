using ModsDude.Server.Domain.Mods;

namespace ModsDude.Server.Domain.Profiles;

public class ModDependency
{
    public required ModVersion ModVersion { get; set; }
    public required bool LockVersion { get; set; }


    public bool CanBeUpgraded()
    {
        return ModVersion.Mod.Versions
            .Any(x => x.SequenceNumber > ModVersion.SequenceNumber);
    }

    public void Upgrade()
    {
        ModVersion = ModVersion.Mod.GetLatestVersion();
    }
}
