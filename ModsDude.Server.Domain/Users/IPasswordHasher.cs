namespace ModsDude.Server.Domain.Users;

public interface IPasswordHasher
{
    PasswordHash Hash(Password password);
    bool VerifyPassword(Password password, PasswordHash passwordHash);
}