namespace ModsDude.Server.Api.Endpoints.Repos;

public record RepoMembershipDto
{
    public required RepoDto Repo { get; init; }
    public required RepoMembershipLevelEnum MembershipLevel { get; init; }
}
