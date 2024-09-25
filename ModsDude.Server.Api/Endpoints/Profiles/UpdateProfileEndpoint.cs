using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class UpdateProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        throw new NotImplementedException();
    }


    private static async Task<Results<Ok<ProfileDto>, BadRequest<CustomProblemDetails>>> Update(
        Guid repoId,
        Guid profileId,
        UpdateProfileRequest request,
        HttpContext httpContext,
        IRepoAuthorizationService repoAuthorizationService,
        IProfileService profileService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), new(repoId), RepoMembershipLevel.Member, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Member));
        }

        var result = await profileService.Update(new RepoId(repoId), new ProfileId(profileId), new ProfileName(request.Name), cancellationToken);

        switch (result)
        {
            case UpdateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(ProfileDto.FromModel(ok.Profile));

            case UpdateProfileResult.NotFound:
                return TypedResults.BadRequest(Problems.NotFound);

            case UpdateProfileResult.NameTaken:
                return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }
        throw new UnreachableException();
    }


    public record UpdateProfileRequest(string Name);
}
