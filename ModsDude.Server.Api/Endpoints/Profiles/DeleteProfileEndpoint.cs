using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Features.Profiles;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Profiles;

public class DeleteProfileEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("repos/{repoId:guid}/profiles/{profileId:guid}", Delete);
    }

    
    private static async Task<Results<Ok, BadRequest<CustomProblemDetails>>> Delete(
        Guid repoId, Guid profileId,
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

        var result = await profileService.Delete(new RepoId(repoId), new ProfileId(profileId), cancellationToken);

        switch (result)
        {
            case DeleteProfileResult.Ok:
                await unitOfWork.CommitAsync(cancellationToken);
                return TypedResults.Ok();

            case DeleteProfileResult.NotFound:
                return TypedResults.BadRequest(Problems.NotFound);
        }
        throw new UnreachableException();
    }
}
