using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record LoginResult(string AccessToken, string RefreshToken);
