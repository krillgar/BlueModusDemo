namespace BlueModusDemo.Middleware;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RedirectHandler _handler;

    public RedirectMiddleware(RequestDelegate next, RedirectHandler handler)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri($"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}");
        request.Method = new HttpMethod(context.Request.Method);

        var response = await _handler.ProcessAsync(request, CancellationToken.None);

        context.Response.StatusCode = (int)response.StatusCode;

        foreach (var header in response.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in response.Content.Headers)
        {
            context.Response.Headers[header.Key] = header.Value.ToArray();
        }

        var content = await response.Content.ReadAsByteArrayAsync();
        await context.Response.Body.WriteAsync(content, 0, content.Length);
    }
}
