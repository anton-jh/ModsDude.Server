using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ModsDude.Server.Api.ErrorHandling;
using ModsDude.Server.Application.Authorization;
using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Api.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeRepoAttribute(
    RepoMembershipLevel minimumMembershipLevel,
    string repoIdParameterName = "repoId")
    : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var repoAuthorizationService = context.HttpContext.RequestServices.GetRequiredService<IRepoAuthorizationService>();

        var repoIdArgument = context.ActionArguments[repoIdParameterName]
            ?? throw new InvalidOperationException($"Could not get RepoId from action parameter '{repoIdParameterName}'");

        var repoId = (Guid)repoIdArgument;

        var isAuthorized = await repoAuthorizationService.AuthorizeAsync(
            context.HttpContext.User.GetUserId(),
            new RepoId(repoId),
            minimumMembershipLevel,
            context.HttpContext.RequestAborted);

        if (!isAuthorized)
        {
            context.Result = new ObjectResult(Problems.InsufficientRepoAccess(minimumMembershipLevel))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        await next();
    }
}
