using System.Reflection;

namespace ModsDude.Server.Api.Endpoints;

public interface IEndpoint
{
    void Map(IEndpointRouteBuilder builder);
}


public static class EndpointMappingExtensions
{
    public static IEndpointRouteBuilder MapAllEndpointsFromAssembly(this IEndpointRouteBuilder builder, Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Except([typeof(IEndpoint)])
            .Where(x => x.IsAssignableTo(typeof(IEndpoint)));

        foreach (var type in types)
        {
            var instance = (IEndpoint)Activator.CreateInstance(type)!;
            instance.Map(builder);
        }

        return builder;
    }
}
