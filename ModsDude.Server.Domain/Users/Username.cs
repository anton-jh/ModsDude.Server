using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Users;
public class Username : ValueOf<string, Username>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException($"{nameof(Username)} cannot be null or whitespace");
        }
    }
}
