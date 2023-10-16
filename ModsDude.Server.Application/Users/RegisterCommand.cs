using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record RegisterCommand(string Username, string Password, string SystemInvite) : IRequest<LoginResult>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginResult>
{
    private readonly UserRegistrator _userRegistrator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LoginService _loginService;
    private readonly ISystemInviteRepository _systemInviteRepository;
    private readonly ITimeService _timeService;


    public RegisterCommandHandler(
        UserRegistrator userRegistrator,
        IUnitOfWork unitOfWork,
        LoginService loginService,
        ISystemInviteRepository systemInviteRepository,
        ITimeService timeService)
    {
        _userRegistrator = userRegistrator;
        _unitOfWork = unitOfWork;
        _loginService = loginService;
        _systemInviteRepository = systemInviteRepository;
        _timeService = timeService;
    }


    public async Task<LoginResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var username = Username.From(request.Username);
        var password = Password.From(request.Password);
        var invite = SystemInviteId.From(request.SystemInvite);

        await UseInvite(invite, cancellationToken);
        await _userRegistrator.RegisterUser(username, password);
        await _unitOfWork.CommitAsync();

        return await _loginService.Login(username, password, cancellationToken);
    }


    private async Task UseInvite(SystemInviteId systemInviteId, CancellationToken cancellationToken)
    {
        var invite = await _systemInviteRepository.GetByIdAsync(systemInviteId, cancellationToken);

        if (invite is null ||
            invite.Expires < _timeService.GetNow() ||
            !invite.UsesLeft.Any())
        {
            throw new InvalidSystemInviteException();
        }

        invite.DeductOneUse();

        if (!invite.UsesLeft.Any())
        {
            _systemInviteRepository.Delete(invite);
        }
    }
}
