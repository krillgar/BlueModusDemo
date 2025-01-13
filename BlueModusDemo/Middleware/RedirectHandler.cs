using BlueModusDemo.Services;
using BlueModusDemo.Services.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace BlueModusDemo.Middleware;

public class RedirectHandler : DelegatingHandler
{
    private readonly ILogger _logger;
    private static IEnumerable<RedirectResult> _routes = Enumerable.Empty<RedirectResult>();
    private readonly static object _lock = new object();

    public RedirectHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory?.CreateLogger<RedirectHandler>() ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public async Task<HttpResponseMessage> ProcessAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await SendAsync(request, cancellationToken);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var originalPath = request.RequestUri;
        var (path, statusCode) = GetRedirectPath(originalPath?.LocalPath ?? string.Empty);

        try
        {
            HttpResponseMessage response;

            if (statusCode == HttpStatusCode.OK)
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successful call to {path}", path);
                }
                else
                {
                    _logger.LogError("Error calling {path}: {reason}", path, response.ReasonPhrase);
                }
            }
            else
            {
                path += originalPath?.Query;
                response = new HttpResponseMessage(statusCode);

                response.Headers.Add(HeaderNames.Location, path);

                _logger.LogInformation("{statusCode} redirection from {old} to {new}.", statusCode, originalPath, path);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to execute {path}: {message}", path, ex.Message);

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError
            };
        }
    }

    private (string, HttpStatusCode) GetRedirectPath(string path)
    {
        var redirect = _routes.FirstOrDefault(route => path.Contains(route.RedirectUrl, StringComparison.InvariantCultureIgnoreCase));
        var statusCode = HttpStatusCode.OK;

        if (redirect != null)
        {
            statusCode = redirect.RedirectType;
            path = redirect.UseRelative ?
                path.Replace(redirect.TargetUrl, redirect.RedirectUrl) :
                redirect.RedirectUrl;
        }

        return (path, statusCode);
    }

    public static async Task PopulateRoutes(IRedirectionService service, ILogger logger)
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
