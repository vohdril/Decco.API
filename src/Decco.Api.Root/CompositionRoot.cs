using Decco.Api.Common;
using Decco.Api.Contracts;
using Decco.Api.DataLayer;
using Decco.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Decco.Api.Root;

public static class CompositionRoot
{
    public static IServiceCollection RegisterDependencies(this IServiceCollection services)
    {
        var assemblies = new[]
        {
            typeof(IService).Assembly,
            typeof(IRepository).Assembly,
            typeof(IAnomaliaService).Assembly,
            typeof(ErrorCodes).Assembly,
            typeof(DeccoDbContext).Assembly
        };

        RegisterByConvention(services, assemblies, typeof(IService), ServiceLifetime.Scoped);
        RegisterByConvention(services, assemblies, typeof(IRepository), ServiceLifetime.Scoped);

        return services;
    }

    private static void RegisterByConvention(
        IServiceCollection services,
        Assembly[] assemblies,
        Type markerInterface,
        ServiceLifetime lifetime)
    {
        foreach (var assembly in assemblies)
        {
            Type[] types;
            try
            {
                types = assembly.GetExportedTypes();
            }
            catch
            {
                continue;
            }

            foreach (var impl in types.Where(t => t.IsClass && !t.IsAbstract && markerInterface.IsAssignableFrom(t)))
            {
                var iface = impl.GetInterfaces()
                    .FirstOrDefault(i => i != markerInterface && markerInterface.IsAssignableFrom(i) && i != impl);

                if (iface != null)
                {
                    services.Add(new ServiceDescriptor(iface, impl, lifetime));
                }
            }
        }
    }
}
