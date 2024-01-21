using ModsDude.Server.Application.Exceptions;
using ModsDude.Server.Domain.Exceptions;
using ModsDude.Server.Domain.Users;
using System.Security.Claims;

namespace ModsDude.Server.Application.Services.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static UserId GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var value = (claimsPrincipal.FindFirst("sub")?.Value)
            ?? throw new NotAuthenticatedException();

        try
        {
            return UserId.From(value);
        }
        catch (DomainValidationException)
        {
            throw new NotAuthenticatedException();
        }
        catch (FormatException)
        {
            throw new NotAuthenticatedException();
        }
    }
}
