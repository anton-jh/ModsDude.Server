using ModsDude.Server.Domain.Common;
using ValueOf;

namespace ModsDude.Server.Domain.Users;

public class RefreshToken
{
    public RefreshToken(UserId userId, RefreshTokenFamilyId familyId, DateTime created, DateTime expires)
    {
        Id = RefreshTokenId.NewId();

        UserId = userId;
        FamilyId = familyId;
        Created = created;
        Expires = expires;
    }


    public RefreshTokenId Id { get; init; }
    public UserId UserId { get; }
    public RefreshTokenFamilyId FamilyId { get; }
    public DateTime Created { get; }
    public DateTime Expires { get; }
}

public class RefreshTokenId : ValueOf<string, RefreshTokenId>
{
    public static RefreshTokenId NewId()
    {
        return From(Guid.NewGuid().ToString());
    }
}

public class RefreshTokenFamilyId : GuidId<RefreshTokenFamilyId> { }
