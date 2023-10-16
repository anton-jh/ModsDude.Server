using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record LoginCommand(string Username, string Password) : IRequest<LoginResult>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly LoginService _loginService;
    private readonly IUnitOfWork _unitOfWork;


    public LoginCommandHandler(LoginService loginService, IUnitOfWork unitOfWork)
    {
        _loginService = loginService;
        _unitOfWork = unitOfWork;
    }


    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Username username = Username.From(request.Username);
        Password password = Password.From(request.Password);

        var result = await _loginService.Login(username, password, cancellationToken);
        await _unitOfWork.CommitAsync();

        return result;
    }
}
