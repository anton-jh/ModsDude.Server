using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Invites;
public class SystemInvite(DateTime expires, SystemInviteUses usesLeft)
{
    public SystemInviteId Id { get; init; } = SystemInviteId.NewId();
    public DateTime Expires { get; } = expires;
    public SystemInviteUses UsesLeft { get; private set; } = usesLeft;


    public void DeductOneUse()
    {
        UsesLeft = UsesLeft.DeductOne();
    }
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
    public bool Any()
    {
        return Value > 0;
    }

    public SystemInviteUses DeductOne()
    {
        if (Value == 0)
        {
            throw new InvalidOperationException($"Cannot deduct from 0 {nameof(SystemInviteUses)}");
        }

        return From(Value - 1);
    }


    protected override void Validate()
    {
        if (Value < 0)
        {
            throw new DomainValidationException($"{nameof(SystemInviteUses)} cannot be negative");
        }
    }
}
