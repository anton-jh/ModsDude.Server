using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Features.Repos;
public class CreateRepoHandler(
    ITimeService timeService,
    IRepoRepository repoRepository)
    : IRequestHandler<CreateRepoCommand, CreateRepoResult>
{
    public async Task<CreateRepoResult> Handle(CreateRepoCommand request, CancellationToken cancellationToken)
    {
        if (await repoRepository.CheckNameIsTaken(request.Name, cancellationToken))
        {
            return new CreateRepoResult.NameTaken();
        }

        var repo = new Repo(request.Name, request.Adapter, timeService.Now());
        repoRepository.SaveNewRepo(repo);

        return new CreateRepoResult.Ok(repo);
    }
}


public record CreateRepoCommand(
    RepoName Name,
    SerializedAdapter Adapter
) : IRequest<CreateRepoResult>;


public abstract record CreateRepoResult()
{
    public record Ok(Repo Repo) : CreateRepoResult;
    public record NameTaken() : CreateRepoResult;
}
