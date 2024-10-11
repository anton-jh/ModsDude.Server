using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Authorization;

public static class AuthorizationUserExtensions
{
    public static Task<FluentAuthorizationBuilder> IsAllowedTo(this User? user)
        => user is not null
            ? Task.FromResult(new FluentAuthorizationBuilder(user))
            : throw new NotAuthenticatedException();

    public static async Task<FluentAuthorizationBuilder> IsAllowedTo(this Task<User?> userTask)
    {
        var user = await userTask
            ?? throw new NotAuthenticatedException();

        return new FluentAuthorizationBuilder(user);
    }
}
