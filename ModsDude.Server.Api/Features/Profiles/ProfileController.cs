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
    public async Task<ActionResult<ProfileDto>> CreateProfile(Guid repoId, CreateProfileRequest request, CancellationToken cancellationToken)
    {
        var repoIdParsed = new RepoId(repoId);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoIdParsed, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Create(repoIdParsed, new ProfileName(request.Name), cancellationToken);

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
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetAll(Guid repoId, CancellationToken cancellationToken)
    {
        var repoIdParsed = new RepoId(repoId);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoIdParsed, RepoMembershipLevel.Guest, cancellationToken))
        {
            return Forbid();
        }

        var profiles = await dbContext.Profiles
            .Where(x => x.RepoId == repoIdParsed)
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(ProfileDto.FromProfile);

        return Ok(dtos);
    }

    [HttpPut("{repoId:guid}/profiles/{profileId:guid}")]
    public async Task<ActionResult<ProfileDto>> Update(Guid repoId, Guid profileId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var repoIdParsed = new RepoId(repoId);
        var profileIdParsed = new ProfileId(profileId);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoIdParsed, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Update(profileIdParsed, new ProfileName(request.Name), cancellationToken);

        switch (result)
        {
            case UpdateProfileResult.NotFound:
                return NotFound();

            case UpdateProfileResult.NameTaken:
                return Conflict();

            case UpdateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ProfileDto.FromProfile(ok.Profile));
        }
        throw new UnreachableException();
    }

    [HttpDelete("{repoId:guid}/profiles/{profileId:guid}")]
    public async Task<ActionResult> Delete(Guid repoId, Guid profileId, CancellationToken cancellationToken)
    {
        var repoIdParsed = new RepoId(repoId);
        var profileIdParsed = new ProfileId(profileId);

        if (!await repoAuthorizationService.AuthorizeAsync(HttpContext.User.GetUserId(), repoIdParsed, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Delete(profileIdParsed, cancellationToken);

        switch (result)
        {
            case DeleteProfileResult.NotFound:
                return NotFound();

            case DeleteProfileResult.Ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok();
        }
        throw new UnreachableException();
    }
}
g