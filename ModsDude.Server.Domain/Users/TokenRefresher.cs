using ModsDude.Server.Domain.Exceptions;

namespace ModsDude.Server.Domain.Users;
public class TokenRefresher
{
    private readonly RefreshTokenFactory _refreshTokenFactory;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtService _jwtService;


    public TokenRefresher(RefreshTokenFactory refreshTokenFactory, IRefreshTokenRepository refreshTokenRepository, IJwtService jwtService)
    {
        _refreshTokenFactory = refreshTokenFactory;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtService = jwtService;
    }


    public async Task<LoginResult?> TryRefreshAsync(RefreshTokenId refreshTokenId, CancellationToken cancellationToken)
    {
        var thisToken = await _refreshTokenRepository.GetAsync(refreshTokenId, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        var latestToken = await _refreshTokenRepository.GetLatestInFamilyAsync(thisToken.FamilyId, cancellationToken)
            ?? throw new InvalidRefreshTokenException();

        if (thisToken.Id != latestToken.Id)
        {
            await _refreshTokenRepository.DeleteFamilyAsync(thisToken.FamilyId, cancellationToken);
            return null;
        }

        var accessToken = _jwtService.GenerateForUser(thisToken.UserId);
        var newRefreshToken = _refreshTokenFactory.Create(thisToken.UserId, thisToken.FamilyId);

        _refreshTokenRepository.Add(newRefreshToken);

        return new LoginResult(accessToken, newRefreshToken.Id.Value);
    }
}
