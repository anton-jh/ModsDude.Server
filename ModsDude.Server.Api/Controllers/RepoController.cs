using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Repos;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;
using System.Diagnostics;

namespace ModsDude.Server.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}")]
public class RepoController(
    IRepoService repoService,
    IUnitOfWork unitOfWork,
    IRepoAuthorizationService repoAuthorizationService,
    ApplicationDbContext dbContext)
    : ControllerBase
{
    [HttpPost("repos")]
    [Authorize(Scopes.Repo.Create)]
    public async Task<ActionResult<RepoDto>> CreateRepo(CreateRepoRequest request, CancellationToken cancellationToken)
    {
        if (request.ModAdapterScript is null && request.SavegameAdapterScript is null)
        {
            return BadRequest("Cannot create repo with both mod- and savegame-adapters null");
        }

        var userId = HttpContext.User.GetUserId();
        var name = new RepoName(request.Name);
        AdapterScript? modAdapter = request.ModAdapterScript is null
            ? null
            : new AdapterScript(request.ModAdapterScript);
        AdapterScript? savegameAdapter = request.SavegameAdapterScript is null
            ? null
            : new AdapterScript(request.SavegameAdapterScript);

        var result = await repoService.Create(name, modAdapter, savegameAdapter, userId, cancellationToken);

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

    [HttpGet("repos")]
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
        var dtos = repos.Select(x => new RepoMembershipDto(
            RepoDto.FromRepo(x.Repo),
            RepoMembershipLevelEnumExtensions.Map(x.Membership.Level)));

        return dtos;
    }

    [HttpPut("repos/{id:guid}")]
    public async Task<ActionResult<RepoDto>> UpdateRepo(Guid id, UpdateRepoRequest request, CancellationToken cancellationToken)
    {
        var repoId = new RepoId(id);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoId, RepoMembershipLevel.Admin, cancellationToken))
        {
            return Forbid();
        }

        var name = new RepoName(request.Name);

        var result = await repoService.Update(repoId, name, cancellationToken);

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

    [HttpDelete("repos/{id:guid}")]
    public async Task<ActionResult> DeleteRepo(Guid id, CancellationToken cancellationToken)
    {
        var repoId = new RepoId(id);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoId, RepoMembershipLevel.Admin, cancellationToken))
        {
            return Forbid();
        }

        var result = await repoService.Delete(repoId, cancellationToken);

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


public record CreateRepoRequest(string Name, string? ModAdapterScript, string? SavegameAdapterScript);
public record UpdateRepoRequest(string Name);

public record RepoDto(Guid Id, string Name, string? ModAdapter, string? SavegameAdapter)
{
    public static RepoDto FromRepo(Repo repo)
    {
        return new(
            repo.Id.Value,
            repo.Name.Value,
            repo.ModAdapter?.Value,
            repo.SavegameAdapter?.Value);
    }
}
public record RepoMembershipDto(RepoDto Repo, RepoMembershipLevelEnum MembershipLevel);
public enum RepoMembershipLevelEnum
{
    Guest,
    Member,
    Admin
}
public static class RepoMembershipLevelEnumExtensions
{
    public static RepoMembershipLevelEnum Map(RepoMembershipLevel source)
    {
        if (source == RepoMembershipLevel.Admin)
        {
            return RepoMembershipLevelEnum.Admin;
        }
        if (source == RepoMembershipLevel.Member)
        {
            return RepoMembershipLevelEnum.Member;
        }
        if (source == RepoMembershipLevel.Guest)
        {
            return RepoMembershipLevelEnum.Guest;
        }

        throw new UnreachableException();
    }
}
