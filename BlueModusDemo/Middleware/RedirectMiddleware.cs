using BlueModusDemo.Services;
using BlueModusDemo.Services.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace BlueModusDemo.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRedirectionService _service;
    private readonly ILogger _logger;
    private static IEnumerable<RedirectResult> _routes = Enumerable.Empty<RedirectResult>();
    private readonly static object _lock = new object();

    public RedirectMiddleware(RequestDelegate next, IRedirectionService service, ILoggerFactory loggerFactory)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = loggerFactory?.CreateLogger<RedirectMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));

        PopulateRoutes().Wait();
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

    public async Task PopulateRoutes()
    {
        var results = await _service.GetRedirectionsAsync();

        var routes = JsonConvert.DeserializeObject<IEnumerable<RedirectResult>>(results) ?? Enumerable.Empty<RedirectResult>();

        lock (_lock)
        {
            _routes = routes;
        }

        _logger.LogInformation("Routes repopulated at {now}.", DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss.fff"));
    }
}
