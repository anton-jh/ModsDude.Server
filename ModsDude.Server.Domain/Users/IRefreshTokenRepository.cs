namespace ModsDude.Server.Domain.Users;
public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);
    Task<RefreshToken?> GetAsync(RefreshTokenId id, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetLatestInFamilyAsync(RefreshTokenFamilyId familyId, CancellationToken cancellationToken = default);
    Task DeleteFamilyAsync(RefreshTokenFamilyId familyId, CancellationToken cancellationToken = default);
}
