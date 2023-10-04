using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Users;
public class Password : ValueOf<string, Password>
{
    public static int MinLength => 8;


    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new InvalidPasswordException();
        }

        if (Value.Length < MinLength)
        {
            throw new InvalidPasswordException();
        }
    }
}
