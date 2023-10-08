using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Domain.Users;
using ModsDude.Server.Persistence.DbContexts;

namespace ModsDude.Server.Persistence.Repositories;
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;


    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public void Add(RefreshToken refreshToken)
    {
        _dbContext.Add(refreshToken);
    }

    public async Task DeleteFamilyAsync(RefreshTokenFamilyId familyId, CancellationToken cancellationToken = default)
    {
        var tokens = await _dbContext.RefreshTokens
            .Where(t => t.FamilyId == familyId)
            .ToListAsync(cancellationToken);

        _dbContext.RefreshTokens
            .RemoveRange(tokens);
    }

    public async Task<RefreshToken?> GetLatestInFamilyAsync(RefreshTokenFamilyId familyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .Where(t => t.FamilyId == familyId)
            .OrderByDescending(t => t.Created)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
