using HotChocolate.Execution.Configuration;
using System.Reflection;

namespace ModsDude.Server.Api.Extensions;

public static class RequestExecutorBuilderExtensions
{
    public static IRequestExecutorBuilder AddTypeExtensionsFromAssembly(this IRequestExecutorBuilder builder, Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetCustomAttribute<ExtendObjectTypeAttribute>() is not null);

        foreach (var t in types)
        {
            builder.AddTypeExtension(t);
        }

        return builder;
    }

    public static IRequestExecutorBuilder AddTypeExtensionsFromAssemblyContaining<T>(this IRequestExecutorBuilder builder)
    {
        return AddTypeExtensionsFromAssembly(builder, typeof(T).Assembly);
    }
}
