using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ValueOf;

namespace ModsDude.Server.Persistence.ValueConverters;
internal class ValueOfValueConverter<TValue, T> : ValueConverter<ValueOf<TValue, T>, TValue>
    where T : ValueOf<TValue, T>, new()
{
    public ValueOfValueConverter()
        : base(
            model => model.Value,
            value => ValueOf<TValue, T>.From(value))
    {
    }
}
