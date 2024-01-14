//using ModsDude.Server.Domain.Users;
//using ModsDude.Server.Persistence.DbContexts;
//using System.IdentityModel.Tokens.Jwt;

//namespace ModsDude.Server.Api.Middleware;

//public class UserCreatingMiddleware(RequestDelegate next)
//{
//    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
//    {
//        var isAuthenticated = (context.User.Identity?.IsAuthenticated ?? false);
//        var subClaim = context.User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

//        if (!isAuthenticated || subClaim is null)
//        {
//            await next(context);
//            return;
//        }

//        var userId = UserId.From(subClaim.Value);
//        var existingUser = await dbContext.Users.FindAsync(subClaim);

//        if (existingUser is not null)
//        {
//            await next(context);
//            return;
//        }

//        var username = Username.From(nameClaim.Value); // todo
//        var newUser = new User(userId, username);

//        dbContext.Users.Add(newUser);

//        await dbContext.SaveChangesAsync();

//        await next(context);
//    }
//}
