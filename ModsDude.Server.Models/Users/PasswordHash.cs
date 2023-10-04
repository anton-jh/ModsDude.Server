using ModsDude.Server.Common.Exceptions;
using ValueOf;

namespace ModsDude.Server.Models.Users;
public class PasswordHash : ValueOf<string, PasswordHash>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException();
        }
    }
}
