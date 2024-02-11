using ModsDude.Server.Domain.Profiles;

namespace ModsDude.Server.Api.Features.Profiles;

public record ProfileDto(
    Guid Id,
    Guid RepoId,
    string Name
)
{
    public static ProfileDto FromProfile(Profile profile)
    {
        return new(
            profile.Id.Value,
            profile.RepoId.Value,
            profile.Name.Value);
    }
}
