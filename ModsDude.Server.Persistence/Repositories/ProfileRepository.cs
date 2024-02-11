using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class ProfileRepository(
    ApplicationDbContext dbContext)
    : IProfileRepository
{
    public void AddNewProfile(Profile profile)
    {
        dbContext.Profiles.Add(profile);
    }

    public Task<bool> CheckNameIsTaken(RepoId repoId, ProfileName name, CancellationToken cancellationToken)
    {
        return dbContext.Profiles
            .Where(x => x.RepoId == repoId)
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<bool> CheckNameIsTaken(ProfileName name, ProfileId except, CancellationToken cancellationToken)
    {
        var profile = await dbContext.Profiles
            .FindAsync([except], cancellationToken)
            ?? throw new ArgumentException("No profile with provided id exists", nameof(except));

        return await dbContext.Profiles
            .Where(x => x.RepoId == profile.RepoId)
            .AnyAsync(x => x.Name == name && x.Id != except, cancellationToken);
    }

    public void Delete(Profile profile)
    {
        dbContext.Profiles
            .Remove(profile);
    }

    public async Task<Profile?> GetById(ProfileId id, CancellationToken cancellationToken)
    {
        return await dbContext.Profiles
            .FindAsync([id], cancellationToken);
    }
}
