using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Invites;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record RegisterUserCommand(string Username, string Password, string SystemInvite) : IRequest;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly UserRegistrator _userRegistrator;
    private readonly IUnitOfWork _unitOfWork;


    public RegisterUserCommandHandler(UserRegistrator userRegistrator, IUnitOfWork unitOfWork)
    {
        _userRegistrator = userRegistrator;
        _unitOfWork = unitOfWork;
    }


    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Username username = Username.From(request.Username);
        Password password = Password.From(request.Password);
        SystemInvite systemInvite = SystemInvite.From(request.SystemInvite);

        // TODO: Validate systemInvite against database
        await _userRegistrator.RegisterUser(username, password);
        await _unitOfWork.CommitAsync(cancellationToken);
    }
}
