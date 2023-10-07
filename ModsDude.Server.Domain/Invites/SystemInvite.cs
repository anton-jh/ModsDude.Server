using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Invites;
public class SystemInvite
{
    public SystemInvite(DateTime expires, SystemInviteUses usesLeft)
    {
        Id = SystemInviteId.NewId();
        Expires = expires;
        UsesLeft = usesLeft;
    }


    public SystemInviteId Id { get; init; }
    public DateTime Expires { get; }
    public SystemInviteUses UsesLeft { get; }
}

public class SystemInviteId : ValueOf<string, SystemInviteId>
{
    public static SystemInviteId NewId()
    {
        return From(Guid.NewGuid().ToString());
    }
}

public class SystemInviteUses : ValueOf<int, SystemInviteUses>
{
    protected override void Validate()
    {
        if (Value < 0)
        {
            throw new DomainValidationException($"{nameof(SystemInviteUses)} cannot be negative");
        }
    }
}
