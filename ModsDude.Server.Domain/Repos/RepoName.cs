using ModsDude.Server.Domain.Exceptions;
using ValueOf;

namespace ModsDude.Server.Domain.Repos;
public class RepoName : ValueOf<string, RepoName>
{
    protected override void Validate()
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            throw new DomainValidationException($"{nameof(RepoName)} cannot be empty or whitespace");
        }
    }
}
