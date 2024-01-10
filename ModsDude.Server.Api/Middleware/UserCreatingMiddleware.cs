using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ModsDude.Server.Api.Middleware;

public class UserCreatingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var isAuthenticated = (context.User.Identity?.IsAuthenticated ?? false);
        var subClaim = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
        var nameClaim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);

        if (isAuthenticated && subClaim is not null && nameClaim is not null)
        {
            var userId = UserId.From(subClaim.Value);
            var existingUser = await dbContext.Users.FindAsync(subClaim);

            if (existingUser is null)
            {
                var username = Username.From(nameClaim.Value);
                var newUser = new User(userId, username);

                dbContext.Users.Add(newUser);

                await dbContext.SaveChangesAsync();
            }
        }

        await next(context);
    }
}
