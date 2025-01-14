using BlueModusDemo.Options;
using BlueModusDemo.Services;
using BlueModusDemo.Services.Models;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace BlueModusDemo.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private static IEnumerable<RedirectResult> _routes = Enumerable.Empty<RedirectResult>();
    private readonly static object _lock = new object();

    public RedirectMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = loggerFactory?.CreateLogger<RedirectMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalPath = context.Request.Path;
        var (path, statusCode) = GetRedirectPath(originalPath);

        if (statusCode == HttpStatusCode.OK)
        {
            await _next(context);

            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                _logger.LogInformation("No redirection needed to {path}.", originalPath);
            }
            else
            {
                _logger.LogError("{code} error when calling {path}.", context.Response.StatusCode, originalPath);
            }
        }
        else
        {
            context.Response.Headers[HeaderNames.Location] = path;
            context.Response.StatusCode = (int)statusCode;

            _logger.LogInformation("{code} redirection from {original} to {new}.", statusCode, originalPath, path);

            return;
        }
    }
    private (string, HttpStatusCode) GetRedirectPath(PathString path)
    {
        var redirect = _routes.FirstOrDefault(route => path.StartsWithSegments(route.RedirectUrl, StringComparison.InvariantCultureIgnoreCase));
        var statusCode = HttpStatusCode.OK;

        if (redirect != null)
        {
            statusCode = redirect.RedirectType;
            path = redirect.UseRelative ?
                path.ToString().Replace(redirect.RedirectUrl, redirect.TargetUrl) :
                redirect.TargetUrl;
        }

        return (path, statusCode);
    }

    public static void ConfigureRoutes(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var thread = new Thread(new ThreadStart(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using var scope = provider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IRedirectionService>();
                var factory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();

                await PopulateRoutes(service, factory.CreateLogger<RedirectMiddleware>());

                var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<RedirectOptions>>();
                await Task.Delay(TimeSpan.FromMinutes(options.Value.RefreshMinutes), cancellationToken);
            }
        }));
        thread.Start();
    }

    private static async Task PopulateRoutes(IRedirectionService service, ILogger logger)
    {
        var results = await service.GetRedirectionsAsync();

        var routes = JsonConvert.DeserializeObject<IEnumerable<RedirectResult>>(results) ?? Enumerable.Empty<RedirectResult>();

        lock (_lock)
        {
            _routes = routes;
        }

        logger.LogInformation("Routes repopulated at {now}.", DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss.fff"));
    }
}
