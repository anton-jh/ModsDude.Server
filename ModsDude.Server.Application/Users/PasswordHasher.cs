using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public class PasswordHasher : IPasswordHasher
{
    public PasswordHash Hash(Password password)
    {
        string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password.Value);

        return PasswordHash.From(hash);
    }

    public bool VerifyPassword(Password password, PasswordHash passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password.Value, passwordHash.Value);
    }
}
