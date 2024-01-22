using MediatR;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Application.Features.Repos;
public record UpdateRepoCommand(RepoId Id, RepoName Name, ClaimsPrincipal ClaimsPrincipal) : IRepoAuthorizedRequest<UpdateRepoResult>
{
    public RepoMembershipLevel MinimumMembershipLevel => RepoMembershipLevel.Admin;

    public Task<RepoId> GetRepoId()
    {
        return Task.FromResult(Id);
    }
}

public class UpdateRepoHandler(
    IRepoRepository repoRepository)
    : IRequestHandler<UpdateRepoCommand, UpdateRepoResult>
{
    public async Task<UpdateRepoResult> Handle(UpdateRepoCommand request, CancellationToken cancellationToken)
    {
        if (await repoRepository.CheckNameIsTaken(request.Name, request.Id, cancellationToken))
        {
            return new UpdateRepoResult.NameTaken();
        }

        var repo = await repoRepository.GetById(request.Id);
        if (repo is null)
        {
            return new UpdateRepoResult.NotFound();
        }

        repo.Name = request.Name;

        return new UpdateRepoResult.Ok(repo);
    }
}

public record UpdateRepoResult
{
    public record Ok(Repo Repo) : UpdateRepoResult;
    public record NotFound : UpdateRepoResult;
    public record NameTaken : UpdateRepoResult;
}
