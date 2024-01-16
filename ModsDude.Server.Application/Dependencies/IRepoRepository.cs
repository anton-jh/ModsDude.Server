using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Dependencies;
public interface IRepoRepository
{
    Task<bool> CheckNameIsTaken(RepoName name, CancellationToken cancellationToken);
    void SaveNewRepo(Repo repo);
}
