namespace ModsDude.Server.Domain.RepoMemberships;
public record RepoMembershipLevel
{
    private RepoMembershipLevel(string value)
    {
        Value = value;
    }


    public string Value { get; }

    public static RepoMembershipLevel Guest { get; } = new GuestLevel();
    public static RepoMembershipLevel Member { get; } = new MemberLevel();
    public static RepoMembershipLevel Admin { get; } = new AdminLevel();


    public static RepoMembershipLevel Parse(string value)
    {
        if (value == Guest.Value)
        {
            return Guest;
        }
        if (value == Member.Value)
        {
            return Member;
        }
        if (value == Admin.Value)
        {
            return Admin;
        }
        throw new ArgumentException($"Invalid {nameof(RepoMembershipLevel)} '{value}'", nameof(value));
    }


    private record GuestLevel : RepoMembershipLevel
    {
        public GuestLevel()
            : base("Guest")
        {
        }
    }

    private record MemberLevel : RepoMembershipLevel
    {
        public MemberLevel()
            : base("Member")
        {
        }
    }

    private record AdminLevel : RepoMembershipLevel
    {
        public AdminLevel()
            : base("Admin")
        {
        }
    }
}
