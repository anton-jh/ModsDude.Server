﻿using ModsDude.Server.Domain.Mods;
using ModsDude.Server.Domain.Profiles;
using ModsDude.Server.Domain.RepoMemberships;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ModsDude.Server.Api.ErrorHandling;

public static class Problems
{
    private const string _typeBaseUri = "https://server.modsdude.com/api/problems/";


    public static CustomProblemDetails NameTaken(string name) => new()
    {
        Type = ProblemType.NameTaken,
        Title = "Name taken",
        Detail = $"A resource with the name '{name}' already exists.",
    };

    public static CustomProblemDetails NotFound => new()
    {
        Type = ProblemType.NotFound,
        Title = "Not found",
        Detail = $"The requested resource does not exist."
    };

    public static CustomProblemDetails ModDependencyExists(Profile profile, Mod mod) => new()
    {
        Type = ProblemType.ModDependencyExists,
        Title = $"The profile '{profile.Id.Value}' already has a dependency on mod '{mod.Id.Value}'"
    };

    public static CustomProblemDetails InsufficientRepoAccess(RepoMembershipLevel minimumLevel)
    {
        var levelText =
            minimumLevel == RepoMembershipLevel.Guest ? "Guest" :
            minimumLevel == RepoMembershipLevel.Member ? "Member" :
            minimumLevel == RepoMembershipLevel.Admin ? "Admin" :
            throw new UnreachableException();

        return new()
        {
            Type = ProblemType.InsufficientRepoAccess,
            Title = "Insufficient Repo access",
            Detail = $"The operation requires a Repo membership level of '{levelText}' or greater."
        };
    }

    public static CustomProblemDetails NotAuthorized => new()
    {
        Type = ProblemType.NotAuthorized,
        Title = "Not authorized",
        Detail = $"You are not authorized to perform this operation."
    };

    public static CustomProblemDetails CannotKickOnlyAdmin => new()
    {
        Type = ProblemType.CannotKickOnlyAdmin,
        Title = "Cannot kick last admin",
        Detail = "You cannot kick the only admin of the repo"
    };

    
    public enum ProblemType
    {
        [EnumMember(Value = _typeBaseUri + "name-taken")]
        NameTaken,

        [EnumMember(Value = _typeBaseUri + "not-found")]
        NotFound,

        [EnumMember(Value = _typeBaseUri + "mod-dependency-exists")]
        ModDependencyExists,

        [EnumMember(Value = _typeBaseUri + "insufficient-repo-access")]
        InsufficientRepoAccess,

        [EnumMember(Value = _typeBaseUri + "not-authorized")]
        NotAuthorized,

        [EnumMember(Value = _typeBaseUri + "cannot-kick-only-admin")]
        CannotKickOnlyAdmin,
    }
}
