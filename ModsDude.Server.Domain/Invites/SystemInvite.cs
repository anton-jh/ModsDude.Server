using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Invites;
public class SystemInvite : ValueOf<string, SystemInvite>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException($"{nameof(SystemInvite)} cannot be empty or whitespace");
        }
    }
}
