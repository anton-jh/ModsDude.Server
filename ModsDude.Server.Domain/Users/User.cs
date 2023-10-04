using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Users;
public class User
{
    public User(Username username, PasswordHash passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;

        Id = UserId.NewId();
    }

    public UserId Id { get; init; }

    public Username Username { get; set; }
    public PasswordHash PasswordHash { get; set; }
}

public class UserId : GuidId<UserId> { }
