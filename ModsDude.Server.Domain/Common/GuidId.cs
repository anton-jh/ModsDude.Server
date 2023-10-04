using ValueOf;

namespace ModsDude.Server.Domain.Common;
public class GuidId<T> : ValueOf<Guid, T>
    where T : ValueOf<Guid, T>, new()
{
    public static T NewId()
    {
        return From(Guid.NewGuid());
    }
}
