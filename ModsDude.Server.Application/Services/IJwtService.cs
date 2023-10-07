using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Services;
public interface IJwtService
{
    string GenerateForUser(UserId userId);
}