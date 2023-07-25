using System.Reflection;

namespace FotoApi.Abstractions.Messaging;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddFotoAppHandlers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var handlers = types.Where(t => t.IsAssignableTo(typeof(IBaseHandler)) && !t.IsInterface && !t.IsAbstract);
        foreach (var handler in handlers)
        {
            services.AddScoped(handler);
            // var interfaces = handler.GetInterfaces();
            // foreach (var @interface in interfaces)
            // {
            //     if (@interface.IsAssignableTo(typeof(IBaseHandler)))
            //     {
            //         services.AddTransient(@interface, handler);
            //     }
            // }
        }

        return services;
    }
    
    public static IServiceCollection AddFotoAppPipelines(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();
        var pipelines = types.Where(t => t.IsAssignableTo(typeof(IPipeline)) && !t.IsInterface && !t.IsAbstract);
        foreach (var pipeline in pipelines)
        {
            services.AddScoped(pipeline);
        }

        return services;
    }
}