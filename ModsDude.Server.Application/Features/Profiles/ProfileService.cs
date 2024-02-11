using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Features.Profiles;
public class ProfileService(
    IProfileRepository profileRepository,
    ITimeService timeService) : IProfileService
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

    public async Task<UpdateProfileResult> Update(ProfileId id, ProfileName name, CancellationToken cancellationToken)
    {
        if (await profileRepository.CheckNameIsTaken(name, id, cancellationToken))
        {
            return new UpdateProfileResult.NameTaken();
        }

        var profile = await profileRepository.GetById(id, cancellationToken);
        if (profile is null)
        {
            return new UpdateProfileResult.NotFound();
        }

        profile.Name = name;

        return new UpdateProfileResult.Ok(profile);
    }

    public async Task<DeleteProfileResult> Delete(ProfileId id, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetById(id, cancellationToken);
        if (profile is null)
        {
            return new DeleteProfileResult.NotFound();
        }

        profileRepository.Delete(profile);

        return new DeleteProfileResult.Ok();
    }
}


public abstract record CreateProfileResult()
{
    public record Ok(Profile Profile) : CreateProfileResult;
    public record NameTaken : CreateProfileResult;
}

public abstract record UpdateProfileResult()
{
    public record Ok(Profile Profile) : UpdateProfileResult;
    public record NameTaken : UpdateProfileResult;
    public record NotFound : UpdateProfileResult;
}

public abstract record DeleteProfileResult()
{
    public record Ok : DeleteProfileResult;
    public record NotFound : DeleteProfileResult;
}
