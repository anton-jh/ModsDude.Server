using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Profiles;
public interface IProfileRepository
{
    Task<bool> CheckNameIsTaken(RepoId repoId, ProfileName name, CancellationToken cancellationToken);
    Task<bool> CheckNameIsTaken(ProfileName name, ProfileId except, CancellationToken cancellationToken);
    Task<Profile?> GetById(ProfileId id, CancellationToken cancellationToken);
    void AddNewProfile(Profile profile);
    void Delete(Profile profile);
}
