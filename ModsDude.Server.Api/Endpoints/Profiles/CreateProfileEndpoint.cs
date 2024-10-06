using Microsoft.AspNetCore.Http.HttpResults;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Dtos;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Repositories;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

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
        IProfileRepository profileRepository,
        ITimeService timeService,
        IUnitOfWork unitOfWork,
        CancellationToken cancellationToken)
    {
        if (!await repoAuthorizationService.AuthorizeAsync(httpContext.User.GetUserId(), new(repoId), RepoMembershipLevel.Member, cancellationToken))
        {
            return TypedResults.BadRequest(Problems.InsufficientRepoAccess(RepoMembershipLevel.Member));
        }

        if (await profileRepository.CheckNameIsTaken(new RepoId(repoId), new ProfileName(request.Name), cancellationToken))
        {
            return TypedResults.BadRequest(Problems.NameTaken(request.Name));
        }

        var profile = new Profile(new RepoId(repoId), new ProfileName(request.Name), timeService.Now());
        profileRepository.AddNewProfile(profile);
        await unitOfWork.CommitAsync(cancellationToken);

        return TypedResults.Ok(ProfileDto.FromModel(profile));
    }


    public record CreateProfileRequest(string Name);
}
