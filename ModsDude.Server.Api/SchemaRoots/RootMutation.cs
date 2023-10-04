namespace ModsDude.Server.Api.Schema;

public class RootMutation
{
    public string Test(string parameter)
    {
        return $"You said '{parameter}', noted.";
    }
}
