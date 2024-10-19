﻿using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Security.Claims;

namespace ModsDude.Server.Api.Endpoints.Mods;

public class RegisterModEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos/{repoId:guid}/mods", RegisterMod);
    }


    public async Task<Results<Ok<ModDto>, BadRequest<CustomProblemDetails>>> RegisterMod(
        Guid repoId,
        RegisterModRequest request,
        ClaimsPrincipal claimsPrincipal,
        IUserRepository userRepository,
        IStorageService storageService,
        IModRepository modRepository,
        ITimeService timeService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        var authResult = await userRepository.GetByIdAsync(claimsPrincipal.GetUserId(), cancellationToken)
            .CheckIsAllowedTo(x => x
                .AccessRepoAtLevel(new RepoId(repoId), RepoMembershipLevel.Member))
            .MapToBadRequest();
        if (authResult is not null)
        {
            return authResult;
        }

        if (await storageService.CheckIfModExists(new ModId(request.ModId), new ModVersionId(request.VersionId), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.ModVersionAlreadyExists(new RepoId(repoId), new ModId(request.ModId), new ModVersionId(request.VersionId)));
        }

        var mod = await modRepository.GetMod(new RepoId(repoId), new ModId(request.ModId), cancellationToken);

        if (mod is null)
        {
            mod = new Mod(
                new RepoId(repoId),
                new ModId(request.ModId),
                new ModVersionId(request.VersionId),
                request.Attributes.Select(ModAttributeDto.ToModel),
                timeService.Now(),
                request.Description,
                request.DisplayName);

            modRepository.AddNewMod(mod);
        }
        else
        {
            mod.AddVersion(
                new ModVersionId(request.VersionId),
                request.Attributes.Select(ModAttributeDto.ToModel),
                timeService.Now(),
                request.Description,
                request.DisplayName);
        }

        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ModDto.FromModel(mod));
    }


    public record RegisterModRequest(
        string ModId,
        string VersionId,
        string DisplayName,
        string Description,
        IEnumerable<ModAttributeDto> Attributes);
}
