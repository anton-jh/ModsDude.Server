using Microsoft.Extensions.Options;
using ModsDude.Server.Domain.Common;

namespace ModsDude.Server.Domain.Users;
public class RefreshTokenFactory
{
    private readonly ITimeService _timeService;
    private readonly UsersOptions _options;

    public RefreshTokenFactory(ITimeService timeService, IOptions<UsersOptions> options)
    {
        _timeService = timeService;
        _options = options.Value;
    }


    public RefreshToken Create(UserId userId, RefreshTokenFamilyId? familyId)
    {
        return new RefreshToken(
            userId,
            familyId ?? RefreshTokenFamilyId.NewId(),
            _timeService.GetNow(),
            _timeService.GetNow().AddSeconds(_options.RefreshTokenLifetimeInSeconds));
    }
}
