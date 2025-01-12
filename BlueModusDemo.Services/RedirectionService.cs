using BlueModusDemo.Services.Models;
using Newtonsoft.Json;
using System.Net;

namespace BlueModusDemo.Services;

/// <summary>
/// A class to retrieve the redirection URLs for the demo application.
/// </summary>
public class RedirectionService : IRedirectionService
{
    private readonly IReadOnlyCollection<RedirectResult> _redirectResults = new[] {
        new RedirectResult() {
            RedirectUrl = "/campaignA",
            TargetUrl = "/campaigns/targetcampaign",
            RedirectType = HttpStatusCode.Found,
            UseRelative = false
        },
        new RedirectResult()
        {
            RedirectUrl = "/campaignB",
            TargetUrl = "/campaigns/targetcampaign/channelB",
            RedirectType = HttpStatusCode.Found,
            UseRelative = false
        },
        new RedirectResult() {
            RedirectUrl = "/product-directory",
            TargetUrl = "/products",
            RedirectType = HttpStatusCode.MovedPermanently,
            UseRelative = true
        }
    };

    /// <inheritdoc />
    public Task<string> GetRedirections()
    {
        return Task.FromResult(JsonConvert.SerializeObject(_redirectResults));
    }
}
