using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Dependencies;
public interface IRepoRepository
{
    Task<bool> CheckNameIsTaken(RepoName name, CancellationToken cancellationToken);
    Task<Repo?> GetById(RepoId repoId);
    void AddNewRepo(Repo repo);
    void Delete(Repo repo);
}
