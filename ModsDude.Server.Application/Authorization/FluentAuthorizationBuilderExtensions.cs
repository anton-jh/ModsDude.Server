using ModsDude.Server.Domain.RepoMemberships;
using ModsDude.Server.Domain.Repos;
using ModsDude.Server.Domain.Users;
using System.Diagnostics;

namespace ModsDude.Server.Application.Authorization;

public static class FluentAuthorizationBuilderExtensions
{
    public static async Task<AuthorizationResult?> GetResult(this Task<FluentAuthorizationBuilder> builderTask)
        => (await builderTask).Result;

    public static async Task<FluentAuthorizationBuilder> AccessRepoAtLevel(this Task<FluentAuthorizationBuilder> builderTask,
        RepoId repoId, RepoMembershipLevel minimumMembershipLevel)
    {
        var builder = await builderTask;

        if (builder.Result is not null)
        {
            return builder;
        }

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

        if (builder.Result is not null)
        {
            return builder;
        }

        var membership = builder.User.RepoMemberships.FirstOrDefault(x => x.RepoId == repoId);

        if (membership is null || membership.Level < level)
        {
            builder.Result = new AuthorizationResult.InsufficientRepoAccess(membership?.Level, level);
        }

        return builder;
    }

    public static async Task<FluentAuthorizationBuilder> ChangeOthersMembership(this Task<FluentAuthorizationBuilder> builderTask,
        RepoMembership subjectMembership)
    {
        var builder = await builderTask;

        if (builder.Result is not null)
        {
            return builder;
        }

        var membership = builder.User.RepoMemberships.FirstOrDefault(x => x.RepoId == subjectMembership.RepoId);

        var neededLevel = subjectMembership.Level switch
        {
            RepoMembershipLevel.Guest => RepoMembershipLevel.Member,
            RepoMembershipLevel.Member => RepoMembershipLevel.Admin,
            RepoMembershipLevel.Admin => RepoMembershipLevel.Admin,
            _ => throw new UnreachableException("Invalid enum value")
        };

        if (membership is null || membership.Level < neededLevel)
        {
            builder.Result = new AuthorizationResult.InsufficientRepoAccess(membership?.Level, neededLevel);
        }

        return builder;
    }
}   
