using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Domain.Profiles;
public interface IProfileRepository
{
    Task<bool> CheckNameIsTaken(ProfileName name, CancellationToken cancellationToken);
    Task<bool> CheckNameIsTaken(ProfileName name, ProfileId except, CancellationToken cancellationToken);
    Task<Profile?> GetById(ProfileId id);
    void AddNewProfile(Profile profile);
    void Delete(Profile profile);
}
