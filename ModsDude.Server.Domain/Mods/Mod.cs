using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Mods;

public class Mod
{
    public required ModId Id { get; init; }
    public required RepoId Repo { get; init; }

    public required DateTimeOffset Created { get; init; }
}


public readonly record struct ModId(string Value);
