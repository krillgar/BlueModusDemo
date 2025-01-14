using Microsoft.Extensions.DependencyInjection;

namespace BlueModusDemo.Services.Middleware;

public static class ServiceMiddleware
{
    public static IServiceCollection UseServices(this IServiceCollection services)
    {
        services.AddTransient<IRedirectionService, RedirectionService>();

        return services;
    }
}
