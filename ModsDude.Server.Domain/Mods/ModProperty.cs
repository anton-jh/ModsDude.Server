namespace ModsDude.Server.Domain.Mods;
public class ModProperty(
    string key,
    string value)
{
    public required string Key { get; init; } = key;
    public required string Value { get; set; } = value;
}
