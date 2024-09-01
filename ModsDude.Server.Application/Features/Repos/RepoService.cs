using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Features.Repos;
public class RepoService(
    IRepoRepository repoRepository,
    IUserRepository userRepository,
    ITimeService timeService) : IRepoService
{
    public async Task<CreateRepoResult> Create(RepoName name, AdapterScript? modAdapter, AdapterScript? savegameAdapter, UserId createdBy, CancellationToken cancellationToken)
    {
        if (await repoRepository.CheckNameIsTaken(name, cancellationToken))
        {
            return new CreateRepoResult.NameTaken();
        }

        var repo = new Repo(name, modAdapter, savegameAdapter, timeService.Now());
        repoRepository.AddNewRepo(repo);

        var user = await userRepository.GetByIdAsync(createdBy, cancellationToken)
            ?? throw new Exception("Error while creating repo: Cannot get user");

        user.SetMembershipLevel(repo.Id, RepoMembershipLevel.Admin);

        return new CreateRepoResult.Ok(repo);
    }

    public async Task<UpdateRepoResult> Update(RepoId id, RepoName name, CancellationToken cancellationToken)
    {
        if (await repoRepository.CheckNameIsTaken(name, id, cancellationToken))
        {
            return new UpdateRepoResult.NameTaken();
        }

        var repo = await repoRepository.GetById(id);
        if (repo is null)
        {
            return new UpdateRepoResult.NotFound();
        }

        repo.Name = name;

        return new UpdateRepoResult.Ok(repo);
    }

    public async Task<DeleteRepoResult> Delete(RepoId id, CancellationToken cancellationToken)
    {
        var repo = await repoRepository.GetById(id);
        if (repo is null)
        {
            return new DeleteRepoResult.NotFound();
        }

        repoRepository.Delete(repo);

        return new DeleteRepoResult.Ok();
    }
}


public abstract record CreateRepoResult()
{
    public record Ok(Repo Repo) : CreateRepoResult;
    public record NameTaken() : CreateRepoResult;
}

public abstract record DeleteRepoResult
{
    public record Ok : DeleteRepoResult;
    public record NotFound : DeleteRepoResult;
}

public abstract record UpdateRepoResult
{
    public record Ok(Repo Repo) : UpdateRepoResult;
    public record NotFound : UpdateRepoResult;
    public record NameTaken : UpdateRepoResult;
}
