using BlueModusDemo.Options;

namespace BlueModusDemo.Middleware;

public static class UseRedirectMiddleware
{
    public static IServiceCollection UseCustomRedirection(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<RedirectOptions>(configuration.GetSection(RedirectOptions.Redirect));

        return services;
    }
}
