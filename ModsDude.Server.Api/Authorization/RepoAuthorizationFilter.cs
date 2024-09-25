//using Microsoft.AspNetCore.Mvc;
//using ModsDude.Server.Api.ErrorHandling;
//using ModsDude.Server.Application.Authorization;
//using ModsDude.Server.Domain.RepoMemberships;
//using ModsDude.Server.Domain.Repos;

//namespace ModsDude.Server.Api.Authorization;

//public class RepoAuthorizationFilter(RepoMembershipLevel minimumLevel, string repoIdParameterName = "repoId")
//    : IEndpointFilter
//{
//    public ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
//    {
//        var repoAuthorizationService = context.HttpContext.RequestServices.GetRequiredService<IRepoAuthorizationService>();

//        var repoIdArgument = context.HttpContext.Request
//            ?? throw new InvalidOperationException($"Could not get RepoId from action parameter '{repoIdParameterName}'");

//        var repoId = (Guid)repoIdArgument;

//        var isAuthorized = await repoAuthorizationService.AuthorizeAsync(
//            context.HttpContext.User.GetUserId(),
//            new RepoId(repoId),
//            minimumMembershipLevel,
//            context.HttpContext.RequestAborted);

//        if (!isAuthorized)
//        {
//            context.Result = new ObjectResult(Problems.InsufficientRepoAccess(minimumMembershipLevel))
//            {
//                StatusCode = StatusCodes.Status403Forbidden
//            };
//        }

//        await next();
//    }
//}

//public static class RepoAuthorizationFilterExtensions
//{
//    public static RouteHandlerBuilder AuthorizeRepo(this RouteHandlerBuilder builder, RepoMembershipLevel minimumLevel)
//    {
//        builder.AddEndpointFilter(new RepoAuthorizationFilter(minimumLevel));

//        return builder;
//    }
//}
