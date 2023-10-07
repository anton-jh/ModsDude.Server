using MediatR;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly LoginService _loginService;


    public LoginCommandHandler(LoginService loginService)
    {
        _loginService = loginService;
    }


    public Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Username username = Username.From(request.Username);
        Password password = Password.From(request.Password);

        return _loginService.Login(username, password, cancellationToken);
    }
}
