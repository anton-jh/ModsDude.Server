using ModsDude.Server.Models.Common;

namespace ModsDude.Server.Models.Users;
public class User
{
    public required UserId Id { get; init; }

    public required Username Username { get; set; }
    public required PasswordHash PasswordHash { get; set; }
}

public class UserId : GuidId<UserId> { }
