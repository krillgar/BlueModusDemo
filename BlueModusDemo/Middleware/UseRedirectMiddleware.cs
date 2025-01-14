using BlueModusDemo.Options;
using BlueModusDemo.Services.Middleware;

namespace BlueModusDemo.Middleware;

public static class UseRedirectMiddleware
{
    public static IServiceCollection UseCustomRedirection(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .Configure<RedirectOptions>(configuration.GetSection(RedirectOptions.Redirect))
            .UseServices();

        return services;
    }
}
