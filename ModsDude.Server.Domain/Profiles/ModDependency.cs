using ModsDude.Server.Domain.Mods;

namespace ModsDude.Server.Domain.Profiles;

public class ModDependency
{
    public required ModId ModId { get; init; }
    public required ModVersionId ModVersionId { get; set; }
    public required bool LockVersion { get; set; }
}
