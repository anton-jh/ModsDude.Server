using ModsDude.Server.Domain.RepoMemberships;
using System.Diagnostics;

namespace ModsDude.Server.Api.Endpoints.Repos;

public enum RepoMembershipLevelEnum
{
    Guest,
    Member,
    Admin
}


public static class RepoMembershipLevelEnumExtensions
{
    public static RepoMembershipLevelEnum Map(RepoMembershipLevel source)
    {
        if (source == RepoMembershipLevel.Admin)
        {
            return RepoMembershipLevelEnum.Admin;
        }
        if (source == RepoMembershipLevel.Member)
        {
            return RepoMembershipLevelEnum.Member;
        }
        if (source == RepoMembershipLevel.Guest)
        {
            return RepoMembershipLevelEnum.Guest;
        }

        throw new UnreachableException();
    }
}
