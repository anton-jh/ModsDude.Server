using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Application.Exceptions;
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


    public RegisterCommandHandler(
        UserRegistrator userRegistrator,
        IUnitOfWork unitOfWork,
        LoginService loginService,
        ISystemInviteRepository systemInviteRepository)
    {
        _userRegistrator = userRegistrator;
        _unitOfWork = unitOfWork;
        _loginService = loginService;
        _systemInviteRepository = systemInviteRepository;
    }


    public async Task<LoginResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var username = Username.From(request.Username);
        var password = Password.From(request.Password);

        await ValidateSystemInvite(SystemInviteId.From(request.SystemInvite), cancellationToken);
        await _userRegistrator.RegisterUser(username, password);
        await _unitOfWork.CommitAsync(cancellationToken);

        return await _loginService.Login(username, password, cancellationToken);
    }


    private async Task ValidateSystemInvite(SystemInviteId systemInviteId, CancellationToken cancellationToken)
    {
        var systemInvite = await _systemInviteRepository.GetByIdAsync(systemInviteId, cancellationToken)
            ?? throw new InvalidSystemInviteException();

        _systemInviteRepository.Delete(systemInvite);
    }
}
