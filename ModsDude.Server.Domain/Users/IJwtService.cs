namespace ModsDude.Server.Domain.Users;
public interface IJwtService
{
    string GenerateForUser(UserId userId);
}
