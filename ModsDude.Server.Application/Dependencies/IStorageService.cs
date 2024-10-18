using ModsDude.Server.Domain.Mods;

namespace ModsDude.Server.Application.Dependencies;
public interface IStorageService
{
    Task<bool> CheckIfModExists(ModId modId, ModVersionId versionId, CancellationToken cancellationToken);
}
