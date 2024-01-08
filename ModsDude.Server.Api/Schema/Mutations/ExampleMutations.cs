using ModsDude.Server.Api.Schema.Roots;

namespace ModsDude.Server.Api.Schema.Mutations;

[ExtendObjectType(typeof(RootMutation))]
public class ExampleMutations
{
    //Saved as examples
    //
    //[UseMutationConvention]
    //[Error(typeof(UsernameTakenException))]
    //[Error(typeof(InvalidSystemInviteException))]
    //public Task<LoginResult> RegisterUser(
    //    string username, string password, string systemInvite,
    //    [Service] ISender sender)
    //{
    //    return sender.Send(new RegisterCommand(username, password, systemInvite));
    //}

    //[UseMutationConvention]
    //[Error(typeof(UserNotFoundException))]
    //[Error(typeof(WrongPasswordException))]
    //public Task<LoginResult> Login(
    //    string username, string password,
    //    [Service] ISender sender)
    //{
    //    return sender.Send(new LoginCommand(username, password));
    //}

    //[Error(typeof(InvalidRefreshTokenException))]
    //public Task<LoginResult> Refresh(
    //    string refreshToken,
    //    [Service] ISender sender)
    //{
    //    return sender.Send(new RefreshCommand(refreshToken));
    //}
}
