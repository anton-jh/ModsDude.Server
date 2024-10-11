using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;

namespace ModsDude.Server.Application.Authorization;

public static class FluentAuthorizationBuilderExtensions
{
    public static async Task<AuthorizationResult?> GetResult(this Task<FluentAuthorizationBuilder> builderTask)
        => (await builderTask).Result;

    public static async Task<FluentAuthorizationBuilder> AccessRepoAtLevel(this Task<FluentAuthorizationBuilder> builderTask,
        RepoId repoId, RepoMembershipLevel minimumMembershipLevel)
    {
        var builder = await builderTask;

        var membership = builder.User.RepoMemberships.FirstOrDefault(x => x.RepoId == repoId);

        if (membership is null || membership.Level < minimumMembershipLevel)
        {
            builder.Result = new AuthorizationResult.InsufficientRepoAccess(membership?.Level, minimumMembershipLevel);
        }

        return builder;
    }

    public static async Task<FluentAuthorizationBuilder> GrantAccessToRepo(this Task<FluentAuthorizationBuilder> builderTask,
        RepoId repoId, RepoMembershipLevel level)
    {
        var builder = await builderTask;

        var membership = builder.User.RepoMemberships.FirstOrDefault(x => x.RepoId == repoId);

        if (membership is null || membership.Level < level)
        {
            builder.Result = new AuthorizationResult.InsufficientRepoAccess(membership?.Level, level);
        }

        return builder;
    }
}   
