using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Features.Profiles;
public class ProfileService(
    IProfileRepository profileRepository,
    IModRepository modRepository,
    ITimeService timeService)
    : IProfileService
{
    public async Task<CreateProfileResult> Create(RepoId repoId, ProfileName name, CancellationToken cancellationToken)
    {
        if (await profileRepository.CheckNameIsTaken(repoId, name, cancellationToken))
        {
            return new CreateProfileResult.NameTaken();
        }

        var profile = new Profile(repoId, name, timeService.Now());
        profileRepository.AddNewProfile(profile);

        return new CreateProfileResult.Ok(profile);
    }

    public async Task<UpdateProfileResult> Update(RepoId repoId, ProfileId id, ProfileName name, CancellationToken cancellationToken)
    {
        if (await profileRepository.CheckNameIsTaken(repoId, name, id, cancellationToken))
        {
            return new UpdateProfileResult.NameTaken();
        }

        var profile = await profileRepository.GetById(repoId, id, cancellationToken);
        if (profile is null)
        {
            return new UpdateProfileResult.NotFound();
        }

        profile.Name = name;

        return new UpdateProfileResult.Ok(profile);
    }

    public async Task<DeleteProfileResult> Delete(RepoId repoId, ProfileId id, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetById(repoId, id, cancellationToken);
        if (profile is null)
        {
            return new DeleteProfileResult.NotFound();
        }

        profileRepository.Delete(profile);

        return new DeleteProfileResult.Ok();
    }

    public async Task<AddModDependencyResult> AddModDependency(
        RepoId repoId,
        ProfileId profileId,
        ModId modId,
        ModVersionId modVersionId,
        bool lockVersion,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetById(repoId, profileId, cancellationToken);

        if (profile is null)
        {
            return new AddModDependencyResult.ProfileNotFound();
        }

        var modVersion = await modRepository.GetModVersion(repoId, modId, modVersionId, cancellationToken);

        if (modVersion is null)
        {
            return new AddModDependencyResult.ModNotFound();
        }

        if (profile.ModDependencies.Any(x => x.ModVersion.Mod == modVersion.Mod))
        {
            return new AddModDependencyResult.AlreadyExists();
        }

        var newDependency = profile.AddDependency(modVersion, lockVersion);

        return new AddModDependencyResult.Ok(newDependency);
    }

    public async Task<UpdateModDependencyResult> UpdateModDependency(
        RepoId repoId,
        ProfileId profileId,
        ModId modId,
        ModVersionId modVersionId,
        bool lockVersion,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetById(repoId, profileId, cancellationToken);
        if (profile is null)
        {
            return new UpdateModDependencyResult.ProfileNotFound();
        }

        var dependency = profile.ModDependencies.FirstOrDefault(x => x.ModVersion.Mod.Id == modId);
        if (dependency is null)
        {
            return new UpdateModDependencyResult.DependencyNotFound();
        }

        if (modVersionId != dependency.ModVersion.Id)
        {
            if (!dependency.ModVersion.Mod.Versions.Any(x => x.Id == modVersionId))
            {
                return new UpdateModDependencyResult.ModVersionNotFound();
            }

            dependency.ChangeVersion(modVersionId);
        }

        dependency.LockVersion = lockVersion;
        
        return new UpdateModDependencyResult.Ok(dependency);
    }

    public async Task<DeleteModDependencyResult> DeleteModDependency(RepoId repoId, ProfileId profileId, ModId modId, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetById(repoId, profileId, cancellationToken);
        if (profile is null)
        {
            return new DeleteModDependencyResult.ProfileNotFound();
        }

        var dependency = profile.ModDependencies.FirstOrDefault(x => x.ModVersion.Mod.Id == modId);
        if (dependency is null)
        {
            return new DeleteModDependencyResult.DependencyNotFound();
        }

        profile.DeleteDependency(dependency);

        return new DeleteModDependencyResult.Ok();
    }
}
