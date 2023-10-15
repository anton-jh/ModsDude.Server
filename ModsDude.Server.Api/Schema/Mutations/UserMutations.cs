using MediatR;
using ModsDude.Server.Api.Schema.Roots;
using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Application.Users;
using ModsDude.Server.Domain.Exceptions;

namespace ModsDude.Server.Api.Schema.Mutations;

[ExtendObjectType(typeof(RootMutation))]
public class UserMutations
{
    [UseMutationConvention]
    [Error(typeof(UsernameTakenException))]
    [Error(typeof(InvalidSystemInviteException))]
    public Task<LoginResult> RegisterUser(
        string username, string password, string systemInvite,
        [Service] ISender sender)
    {
        return sender.Send(new RegisterCommand(username, password, systemInvite));
    }

    [UseMutationConvention]
    [Error(typeof(UserNotFoundException))]
    [Error(typeof(WrongPasswordException))]
    public Task<LoginResult> Login(
        string username, string password,
        [Service] ISender sender)
    {
        return sender.Send(new LoginCommand(username, password));
    }
}
