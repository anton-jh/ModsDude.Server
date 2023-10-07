using ModsDude.Server.Domain.Common;

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

public class RefreshTokenId : GuidId<RefreshTokenId> { }
public class RefreshTokenFamilyId : GuidId<RefreshTokenFamilyId> { }
