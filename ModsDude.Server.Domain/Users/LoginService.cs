using Microsoft.Extensions.Options;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Exceptions;

namespace ModsDude.Server.Domain.Users;
public class LoginService
{
    private readonly RefreshTokenFactory _refreshTokenFactory;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UsersOptions _options;
    private readonly IJwtService _jwtService;
    private readonly ITimeService _timeService;


    public LoginService(
        RefreshTokenFactory refreshTokenFactory,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IOptions<UsersOptions> options,
        IJwtService jwtService,
        ITimeService timeService)
    {
        _refreshTokenFactory = refreshTokenFactory;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _options = options.Value;
        _jwtService = jwtService;
        _timeService = timeService;
    }


    public async Task<LoginResult> Login(Username username, Password password, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken)
            ?? throw new UserNotFoundException();

        if (_passwordHasher.VerifyPassword(password, user.PasswordHash) == false)
        {
            throw new WrongPasswordException();
        }

        var accessToken = _jwtService.GenerateForUser(user.Id);

        var refreshToken = _refreshTokenFactory.Create(user.Id, null);
        _refreshTokenRepository.Add(refreshToken);

        return new LoginResult(accessToken, refreshToken.Id.Value);
    }
}
