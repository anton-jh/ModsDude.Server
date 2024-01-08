using ValueOf;

namespace ModsDude.Server.Domain.Users;
public class User(UserId id, Username username)
{
    public UserId Id { get; private set; } = id;

    public Username Username { get; set; } = username;
}

public class UserId : ValueOf<string, UserId>;
