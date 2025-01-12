using System.Net;

namespace BlueModusDemo.Services.Models;

public record RedirectResult
{
    public string RedirectUrl { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
    public HttpStatusCode RedirectType { get; set; } = HttpStatusCode.OK;
    public bool UseRelative { get; set; } = false;
}
