using MediatR;
using ModsDude.Server.Application.Dependencies;
using ModsDude.Server.Domain.Exceptions;
using ModsDude.Server.Domain.Users;

namespace ModsDude.Server.Application.Users;
public record RefreshCommand(string Token) : IRequest<LoginResult>;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, LoginResult>
{
    private readonly TokenRefresher _tokenRefresher;
    private readonly IUnitOfWork _unitOfWork;


    public RefreshCommandHandler(TokenRefresher tokenRefresher, IUnitOfWork unitOfWork)
    {
        _tokenRefresher = tokenRefresher;
        _unitOfWork = unitOfWork;
    }


    public async Task<LoginResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var result = await _tokenRefresher.TryRefreshAsync(RefreshTokenId.From(request.Token), cancellationToken);
        await _unitOfWork.CommitAsync();

        if (result is null)
        {
            throw new InvalidRefreshTokenException();
        }
        
        return result;
    }
}
