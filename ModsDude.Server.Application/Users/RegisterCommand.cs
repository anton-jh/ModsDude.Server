using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record RegisterCommand(string Username, string Password, string SystemInvite) : IRequest<LoginResult>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, LoginResult>
{
    private readonly UserRegistrator _userRegistrator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly LoginService _loginService;


    public RegisterCommandHandler(UserRegistrator userRegistrator, IUnitOfWork unitOfWork, LoginService loginService)
    {
        _userRegistrator = userRegistrator;
        _unitOfWork = unitOfWork;
        _loginService = loginService;
    }


    public async Task<LoginResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        Username username = Username.From(request.Username);
        Password password = Password.From(request.Password);
        SystemInvite systemInvite = SystemInvite.From(request.SystemInvite);

        // TODO: Validate systemInvite against database
        await _userRegistrator.RegisterUser(username, password);
        await _unitOfWork.CommitAsync(cancellationToken);

        return await _loginService.Login(username, password, cancellationToken);
    }
}
