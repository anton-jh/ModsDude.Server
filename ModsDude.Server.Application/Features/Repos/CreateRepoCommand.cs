using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Features.Repos;
public class CreateRepoHandler(
    ITimeService timeService,
    IRepoRepository repoRepository,
    IUserRepository userRepository)
    : IRequestHandler<CreateRepoCommand, CreateRepoResult>
{
    public async Task<CreateRepoResult> Handle(CreateRepoCommand request, CancellationToken cancellationToken)
    {
        if (await repoRepository.CheckNameIsTaken(request.Name, cancellationToken))
        {
            return new CreateRepoResult.NameTaken();
        }

        var repo = new Repo(request.Name, request.Adapter, timeService.Now());
        repoRepository.AddNewRepo(repo);

        var user = await userRepository.GetByIdAsync(request.CreatedBy, cancellationToken)
            ?? throw new Exception("Error while creating repo: Cannot get user");

        user.SetMembershipLevel(repo.Id, RepoMembershipLevel.Admin);

        return new CreateRepoResult.Ok(repo);
    }
}


public record CreateRepoCommand(
    RepoName Name,
    SerializedAdapter Adapter,
    UserId CreatedBy
) : IRequest<CreateRepoResult>;


public abstract record CreateRepoResult()
{
    public record Ok(Repo Repo) : CreateRepoResult;
    public record NameTaken() : CreateRepoResult;
}
