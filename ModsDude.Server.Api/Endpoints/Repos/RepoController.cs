using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Application.Services.Extensions;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

[ApiController]
[Route("repos")]
public class RepoController(
    ISender sender,
    IUnitOfWork unitOfWork,
    ApplicationDbContext dbContext)
    : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Scopes.Repo.Create)]
    public async Task<ActionResult<RepoDto>> CreateRepo(CreateRepoRequest request, CancellationToken cancellationToken)
    {
        var userId = HttpContext.User.GetUserId();
        var name = RepoName.From(request.Name);
        var serializedAdapter = SerializedAdapter.From(request.SerializedAdapter);

        var result = await sender.Send(new CreateRepoCommand(name, serializedAdapter, userId), cancellationToken);

        switch (result)
        {
            case CreateRepoResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(RepoDto.FromRepo(ok.Repo));

            case CreateRepoResult.NameTaken:
                return Conflict();
        }
        throw new UnreachableException();
    }

    [HttpGet("my-repos")]
    public async Task<IEnumerable<RepoMembershipDto>> GetMyRepos(CancellationToken cancellationToken)
    {
        var userId = HttpContext.User.GetUserId();
        var reposQuery = dbContext.RepoMemberships
        .Where(x => x.UserId == userId)
            .Join(dbContext.Repos, mem => mem.RepoId, repo => repo.Id, (mem, repo) => new
            {
                Repo = repo,
                Membership = mem
            })
            .OrderBy(x => x.Repo.Name);
        var repos = await reposQuery.ToListAsync(cancellationToken);
        var dtos = repos.Select(x => new RepoMembershipDto()
        {
            Repo = RepoDto.FromRepo(x.Repo),
            MembershipLevel = RepoMembershipLevelEnumExtensions.Map(x.Membership.Level)
        });

        return dtos;
    }

    [HttpPost("{id:guid}/update")]
    public async Task<ActionResult<RepoDto>> UpdateRepo(Guid id, UpdateRepoRequest request, CancellationToken cancellationToken)
    {
        var parsedId = RepoId.From(id);
        var name = RepoName.From(request.Name);

        var result = await sender.Send<UpdateRepoResult>(new UpdateRepoCommand(parsedId, name, HttpContext.User), cancellationToken);

        switch (result)
        {
            case UpdateRepoResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(RepoDto.FromRepo(ok.Repo));

            case UpdateRepoResult.NotFound:
                return NotFound();

            case UpdateRepoResult.NameTaken:
                return Conflict();
        }
        throw new UnreachableException();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteRepo(Guid id, CancellationToken cancellationToken)
    {
        var parsedId = RepoId.From(id);

        var result = await sender.Send<DeleteRepoResult>(new DeleteRepoCommand(parsedId, HttpContext.User), cancellationToken);

        switch (result)
        {
            case DeleteRepoResult.Ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok();

            case DeleteRepoResult.NotFound:
                return NotFound();
        }
        throw new UnreachableException();
    }
}
