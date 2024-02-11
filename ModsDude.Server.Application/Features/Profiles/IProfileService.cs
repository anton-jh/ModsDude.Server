using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Features.Profiles;
public interface IProfileService
{
    Task<CreateProfileResult> Create(RepoId repoId, ProfileName name, CancellationToken cancellationToken);
    Task<DeleteProfileResult> Delete(ProfileId id, CancellationToken cancellationToken);
    Task<UpdateProfileResult> Update(ProfileId id, ProfileName name, CancellationToken cancellationToken);
}