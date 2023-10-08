namespace ModsDude.Server.Application.Users;
public class UsersOptions
{
    public required string JwtSigningKey { get; init; }
    public required long JwtLifetimeInSeconds { get; init; }
    public required long RefreshTokenLifetimeInSeconds { get; init; }
}
