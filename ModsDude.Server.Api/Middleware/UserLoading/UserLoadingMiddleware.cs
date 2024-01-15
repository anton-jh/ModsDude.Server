using ModsDude.Server.Api.Auth0.AuthenticationApi;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;
using System.IdentityModel.Tokens.Jwt;

namespace ModsDude.Server.Api.Middleware.CreateUser;

public class UserLoadingMiddleware(
    ApplicationDbContext dbContext,
    Auth0AuthenticationApiClient authenticationApiClient,
    ITimeService timeService)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
        var subClaim = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

        if (!isAuthenticated || subClaim is null)
        {
            await next(context);
            return;
        }

        var userId = UserId.From(subClaim.Value);
        var existingUser = await dbContext.Users.FindAsync(userId);

        if (existingUser is not null)
        {
            if (timeService.Now() - existingUser.LastSeen > TimeSpan.FromHours(1))
            {
                existingUser.LastSeen = timeService.Now();
                await dbContext.SaveChangesAsync();
            }

            await next(context);
            return;
        }

        var userInfo = await authenticationApiClient.GetUserInfo();

        var username = Username.From(userInfo.Name);
        var newUser = new User(userId, username, timeService.Now());

        dbContext.Users.Add(newUser);
        await dbContext.SaveChangesAsync();

        await next(context);
    }
}
