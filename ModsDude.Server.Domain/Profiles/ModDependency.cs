using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Profiles;

public class ModDependency
{
    public required ModVersion ModVersion { get; init; }
    public required bool LockVersion { get; set; }
}
