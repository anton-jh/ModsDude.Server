using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;
using System.Diagnostics;

namespace ModsDude.Server.Api.Features.Profiles;

[ApiController]
public class ProfileController(
    RepoAuthorizationService repoAuthorizationService,
    IProfileService profileService,
    IUnitOfWork unitOfWork,
    ApplicationDbContext dbContext)
    : ControllerBase
{
    [HttpPost("{repoId:guid}/profiles")]
    public async Task<ActionResult> CreateProfile(RepoId repoId, ProfileName name, CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Create(repoId, name, cancellationToken);

        switch (result)
        {
            case CreateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ProfileDto.FromProfile(ok.Profile));

            case CreateProfileResult.NameTaken:
                return Conflict();
        }
        throw new UnreachableException();
    }

    [HttpGet("{repoId:guid}/profiles")]
    public async Task<ActionResult> GetProfiles(RepoId /*todo can i make this work??*/ repoId, CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoId, RepoMembershipLevel.Guest, cancellationToken))
        {
            return Forbid();
        }

        var profiles = await dbContext.Profiles
            .Where(x => x.RepoId == repoId)
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(ProfileDto.FromProfile);

        return Ok(dtos);
    }
}


// todo: impl repository, more?