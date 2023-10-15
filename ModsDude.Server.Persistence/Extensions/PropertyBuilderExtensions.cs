using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModsDude.Server.Domain.Common;
using ModsDude.Server.Persistence.ValueConverters;
using ValueOf;

namespace ModsDude.Server.Persistence.Extensions;
internal static class PropertyBuilderExtensions
{
    public static PropertyBuilder<TProperty> HasValueOfConversion<TValue, TProperty>(this PropertyBuilder<TProperty> builder)
        where TProperty : ValueOf<TValue, TProperty>, new()
    {
        return builder.HasConversion<ValueOfValueConverter<TValue, TProperty>>();
    }

    public static PropertyBuilder<TProperty> HasGuidIdConversion<TProperty>(this PropertyBuilder<TProperty> builder)
    where TProperty : GuidId<TProperty>, new()
    {
        return builder.HasConversion<ValueOfValueConverter<Guid, TProperty>>();
    }
}
