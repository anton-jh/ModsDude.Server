using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Users;
public class PasswordHash : ValueOf<string, PasswordHash>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException($"{nameof(PasswordHash)} cannot be empty or whitespace");
        }
    }
}
