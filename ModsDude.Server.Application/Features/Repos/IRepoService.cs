using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Features.Repos;
public interface IRepoService
{
    Task<CreateRepoResult> CreateRepo(RepoName name, SerializedAdapter adapter, UserId createdBy, CancellationToken cancellationToken);
    Task<DeleteRepoResult> DeleteRepo(RepoId id, CancellationToken cancellationToken);
    Task<UpdateRepoResult> UpdateRepo(RepoId id, RepoName name, CancellationToken cancellationToken);
}