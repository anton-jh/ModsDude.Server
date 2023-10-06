namespace ModsDude.Server.Domain.Exceptions;
public class DomainValidationException : Exception
{
    public DomainValidationException(string developerMessage)
        : base(developerMessage)
    {
    }
}
