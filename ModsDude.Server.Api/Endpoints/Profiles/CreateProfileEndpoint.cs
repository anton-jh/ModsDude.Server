using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class CreateProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapPost("repos/{repoId:guid}/profiles", Create);
    }


    private static async Task<Results<Ok<ProfileDto>, BadRequest<CustomProblemDetails>>> Create(
        Guid repoId,
        CreateProfileRequest request,
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

        var result = await profileService.Create(new(repoId), new(request.Name), cancellationToken);

        switch (result)
        {
            case CreateProfileResult.Ok ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok(ProfileDto.FromModel(ok.Profile));

            case CreateProfileResult.NameTaken:
                return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }
        throw new UnreachableException();
    }


    public record CreateProfileRequest(string Name);
}
