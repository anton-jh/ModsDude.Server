using MediatR;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Application.Features.Repos;
public record DeleteRepoCommand(RepoId Id, ClaimsPrincipal ClaimsPrincipal) : IRepoAuthorizedRequest<DeleteRepoResult>
{
    public RepoMembershipLevel MinimumMembershipLevel => RepoMembershipLevel.Admin;

    public Task<RepoId> GetRepoId()
    {
        return Task.FromResult(Id);
    }
}


public class DeleteRepoHandler(
    IRepoRepository repoRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteRepoCommand, DeleteRepoResult>
{
    public async Task<DeleteRepoResult> Handle(DeleteRepoCommand request, CancellationToken cancellationToken)
    {
        var repo = await repoRepository.GetById(request.Id);
        if (repo is null)
        {
            return new DeleteRepoResult.NotFound();
        }

        repoRepository.Delete(repo);
        await unitOfWork.CommitAsync(cancellationToken);

        return new DeleteRepoResult.Ok();
    }
}


public record DeleteRepoResult
{
    public record Ok : DeleteRepoResult;
    public record NotFound : DeleteRepoResult;
}
