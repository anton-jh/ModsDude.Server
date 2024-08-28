namespace ModsDude.Server.Domain.Mods;
public class ModAttribute(
    string key,
    string? value = null)
{
    public required string Key { get; init; } = key;
    public required string? Value { get; set; } = value;
}
