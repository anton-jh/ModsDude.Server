using Microsoft.Extensions.Options;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Application.Services;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public class LoginService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UsersOptions _options;
    private readonly IJwtService _jwtService;
    private readonly ITimeService _timeService;
    private readonly IUnitOfWork _unitOfWork;


    public LoginService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IOptions<UsersOptions> options,
        IJwtService jwtService,
        ITimeService timeService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _options = options.Value;
        _jwtService = jwtService;
        _timeService = timeService;
        _unitOfWork = unitOfWork;
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

        var refreshToken = new RefreshToken(
            user.Id,
            RefreshTokenFamilyId.NewId(),
            _timeService.GetNow(),
            _timeService.GetNow().AddSeconds(_options.RefreshTokenLifetimeInSeconds));
        _refreshTokenRepository.Add(refreshToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new LoginResult(accessToken, refreshToken.Id.Value.ToString());
    }
}
