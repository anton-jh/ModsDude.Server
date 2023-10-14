using ModsDude.Server.Common.Exceptions;
using ValueOf;

namespace ModsDude.Server.Models.Users;
public class Username : ValueOf<string, Username>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException();
        }
    }
}
