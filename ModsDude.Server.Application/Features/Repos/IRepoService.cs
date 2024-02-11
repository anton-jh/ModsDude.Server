using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Features.Repos;
public interface IRepoService
{
    Task<CreateRepoResult> Create(RepoName name, AdapterScript? modAdapter, AdapterScript? savegameAdapter, UserId createdBy, CancellationToken cancellationToken);
    Task<DeleteRepoResult> Delete(RepoId id, CancellationToken cancellationToken);
    Task<UpdateRepoResult> Update(RepoId id, RepoName name, CancellationToken cancellationToken);
}