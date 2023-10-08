using MediatR;
using ModsDude.Server.Api.Schema.Roots;
using ModsDude.Server.Application.Users;

namespace ModsDude.Server.Api.Schema.Mutations;

[ExtendObjectType(typeof(RootMutation))]
public class UserMutations
{
    [UseMutationConvention]
    public Task<LoginResult> RegisterUser(
        string username, string password, string systemInvite,
        [Service] ISender sender)
    {
        return sender.Send(new RegisterCommand(username, password, systemInvite));
    }
}
