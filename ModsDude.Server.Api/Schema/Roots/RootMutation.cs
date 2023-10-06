namespace ModsDude.Server.Api.Schema.Roots;

public class RootMutation
{
    public string Test(string parameter)
    {
        return $"You said '{parameter}', noted.";
    }
}
