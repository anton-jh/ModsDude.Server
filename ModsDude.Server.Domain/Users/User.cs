using ValueOf;

namespace ModsDude.Server.Domain.Users;
public class User(UserId id, Username username, DateTime created)
{
    public UserId Id { get; private set; } = id;

    public Username Username { get; set; } = username;
    public DateTime Created { get; init; } = created;
    public DateTime LastSeen { get; set; } = created;
    public DateTime ProfileLastUpdated { get; set; } = created;
}

public class UserId : ValueOf<string, UserId>;
