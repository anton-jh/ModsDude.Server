using MediatR;
using ModsDude.Server.Api.Schema.Payloads;
using ModsDude.Server.Api.Schema.Roots;
using ModsDude.Server.Application.Users;

namespace ModsDude.Server.Api.Schema.Mutations;

[ExtendObjectType<RootMutation>]
public class UserMutations
{
    [UseMutationConvention]
    public async Task<LoginPayload> RegisterUser(
        string username, string password, string systemInvite,
        [Service] ISender sender)
    {
        await sender.Send(new RegisterUserCommand(username, password, systemInvite));
        // TODO: Login and return login result
    }
}
