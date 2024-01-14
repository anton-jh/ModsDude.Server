using HotChocolate.Authorization;
using ModsDude.Server.Api.Authorization;
using ModsDude.Server.Api.Schema.Roots;

namespace ModsDude.Server.Api.Schema.Mutations;

[ExtendObjectType(typeof(RootMutation))]
public class RepoMutations
{
    [Authorize(Policy = Scopes.Repo.Create)]
    public Task<string> CreateRepo()
    {
        return Task.FromResult("test");
    }

    [Authorize]
    public Task<string> DoStuff()
    {
        return Task.FromResult("stuff");
    }
}
