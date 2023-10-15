using Microsoft.EntityFrameworkCore;
using ModsDude.Server.Persistence.ValueConverters;
using ValueOf;

namespace ModsDude.Server.Persistence.Extensions;
internal static class ModelConfigurationBuilderExtensions
{
    public static ModelConfigurationBuilder HasValueOfConversion<TValue, TProp>(this ModelConfigurationBuilder builder)
        where TProp : ValueOf<TValue, TProp>, new()
    {
        builder
            .Properties<TProp>()
            .HaveConversion<ValueOfValueConverter<TValue, TProp>>();

        return builder;
    }

    public static ModelConfigurationBuilder HasGuidIdConversion<TId>(this ModelConfigurationBuilder builder)
        where TId : ValueOf<Guid, TId>, new()
    {
        return builder.HasValueOfConversion<Guid, TId>();
    }
}
