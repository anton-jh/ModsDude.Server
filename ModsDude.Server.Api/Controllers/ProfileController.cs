using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Persistence.DbContexts;
using System.Diagnostics;

namespace ModsDude.Server.Api.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{v:apiVersion}")]
public class ProfileController(
    IRepoAuthorizationService repoAuthorizationService,
    IProfileService profileService,
    IUnitOfWork unitOfWork,
    ApplicationDbContext dbContext)
    : RepoAuthorizedController(
        repoAuthorizationService)
{
    [HttpPost("repos/{repoId:guid}/profiles")]
    public async Task<ActionResult<ProfileDto>> CreateProfile(Guid repoId, CreateProfileRequest request, CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Create(new RepoId(repoId), new ProfileName(request.Name), cancellationToken);

        switch (result)
        {
            case CreateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ProfileDto.FromModel(ok.Profile));

            case CreateProfileResult.NameTaken:
                return Conflict();
        }
        throw new UnreachableException();
    }

    [HttpGet("repos/{repoId:guid}/profiles")]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetAll(Guid repoId, CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Guest, cancellationToken))
        {
            return Forbid();
        }

        var profiles = await dbContext.Profiles
            .Where(x => x.RepoId == new RepoId(repoId))
            .ToListAsync(cancellationToken);

        var dtos = profiles.Select(ProfileDto.FromModel);

        return Ok(dtos);
    }

    [HttpPut("repos/{repoId:guid}/profiles/{profileId:guid}")]
    public async Task<ActionResult<ProfileDto>> Update(Guid repoId, Guid profileId, UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Update(new RepoId(repoId), new ProfileId(profileId), new ProfileName(request.Name), cancellationToken);

        switch (result)
        {
            case UpdateProfileResult.NotFound:
                return NotFound();

            case UpdateProfileResult.NameTaken:
                return Conflict();

            case UpdateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ProfileDto.FromModel(ok.Profile));
        }
        throw new UnreachableException();
    }

    [HttpDelete("repos/{repoId:guid}/profiles/{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete(Guid repoId, Guid profileId, CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.Delete(new RepoId(repoId), new ProfileId(profileId), cancellationToken);

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

    [HttpPost("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies")]
    public async Task<ActionResult<ModDependencyDto>> AddModDependency(Guid repoId, Guid profileId, AddModDependencyRequest request, CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.AddModDependency(
            new RepoId(repoId),
            new ProfileId(profileId),
            new ModId(request.ModId),
            new ModVersionId(request.VersionId),
            request.LockVersion,
            cancellationToken);

        switch (result)
        {
            case AddModDependencyResult.ProfileNotFound:
                return NotFound($"No profile '{profileId}' found in repo '{repoId}'");

            case AddModDependencyResult.ModNotFound:
                return UnprocessableEntity($"No mod '{request.ModId}' found in repo '{repoId}'");

            case AddModDependencyResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ModDependencyDto.FromModel(ok.ModDependency));
        }
        throw new UnreachableException();
    }

    [HttpGet("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies")]
    public async Task<ActionResult<IEnumerable<ModDependencyDto>>> GetModDependencies(
        Guid repoId,
        Guid profileId,
        CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Guest, cancellationToken))
        {
            return Forbid();
        }

        var modDependencies = await dbContext.Profiles
            .Where(x => x.RepoId == new RepoId(repoId) && x.Id == new ProfileId(profileId))
            .SelectMany(x => x.ModDependencies)
            .ToListAsync(cancellationToken);

        var dtos = modDependencies.Select(ModDependencyDto.FromModel);

        return Ok(dtos);
    }

    [HttpGet("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies/{modId:guid}")]
    public async Task<ActionResult<ModDependencyDto>> GetModDependency(
        Guid repoId,
        Guid profileId,
        string modId,
        CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var modDependency = await dbContext.Profiles
            .Where(x => x.RepoId == new RepoId(repoId) && x.Id == new ProfileId(profileId))
            .SelectMany(x => x.ModDependencies)
            .FirstOrDefaultAsync(x => x.ModVersion.Mod.Id == new ModId(modId), cancellationToken);

        if (modDependency is null)
        {
            return NotFound("TODO");
        }

        var dto = ModDependencyDto.FromModel(modDependency);

        return Ok(dto);
    }

    [HttpPut("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies/{modId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ModDependencyDto>> UpdateModDependency(
        Guid repoId,
        Guid profileId,
        string modId,
        UpdateModDependencyRequest request,
        CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.UpdateModDependency(
            new RepoId(repoId),
            new ProfileId(profileId),
            new ModId(modId),
            new ModVersionId(request.VersionId),
            request.LockVersion,
            cancellationToken);

        switch (result)
        {
            case UpdateModDependencyResult.DependencyNotFound:
                return NotFound($"No dependency on mod '{modId}' found in profile '{profileId}'");

            case UpdateModDependencyResult.ModVersionNotFound:
                return NotFound($"No version '{request.VersionId}' of mod '{modId}' found in repo '{repoId}'");

            case UpdateModDependencyResult.ProfileNotFound:
                return NotFound($"No profile '{profileId}' found in repo '{repoId}'");

            case UpdateModDependencyResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok(ModDependencyDto.FromModel(ok.ModDependency));
        }
        throw new UnreachableException();
    }

    [HttpDelete("repos/{repoId:guid}/profiles/{profileId:guid}/modDependencies/{modId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteModDependency(
        Guid repoId,
        Guid profileId,
        string modId,
        CancellationToken cancellationToken)
    {
        if (!await AuthorizeForRepoAsync(repoId, RepoMembershipLevel.Member, cancellationToken))
        {
            return Forbid();
        }

        var result = await profileService.DeleteModDependency(
            new RepoId(repoId),
            new ProfileId(profileId),
            new ModId(modId),
            cancellationToken);

        switch (result)
        {
            case DeleteModDependencyResult.ProfileNotFound:
                return NotFound($"No profile '{profileId}' found in repo '{repoId}'");

            case DeleteModDependencyResult.DependencyNotFound:
                return NotFound($"No dependency on mod '{modId}' found in profile '{profileId}'");

            case DeleteModDependencyResult.Ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return Ok();
        }
        throw new UnreachableException();
    }
}

public record CreateProfileRequest(string Name);
public record UpdateProfileRequest(string Name);
public record AddModDependencyRequest(string ModId, string VersionId, bool LockVersion);
public record UpdateModDependencyRequest(string VersionId, bool LockVersion);

public record ModDependencyDto(string ModId, string ModVersionId, bool LockVersion)
{
    public static ModDependencyDto FromModel(ModDependency model)
        => new(model.ModVersion.Mod.Id.Value, model.ModVersion.Id.Value, model.LockVersion);
}
public record ProfileDto(Guid Id, Guid RepoId, string Name)
{
    public static ProfileDto FromModel(Profile profile)
        => new(profile.Id.Value, profile.RepoId.Value, profile.Name.Value);
}
