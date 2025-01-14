using BlueModusDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlueModusDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class CampaignsController : ControllerBase
{
    [HttpGet]
    [Route("targetcampaign")]
    public IActionResult CampaignA()
    {
        return new OkObjectResult(new CampaignDetail
        {
            Id = Guid.NewGuid(),
            Title = "BlueModus",
            Description = "Achieving a new job at BlueModus",
            EstimatedCost = 12345.67,
            Contacts = ["Tom Whittaker", "Dave Bromeland", "Jordan Walters"]
        });
    }

    [HttpGet]
    [Route("targetcampaign/channelB")]
    public IActionResult CampaignB()
    {
        return new OkObjectResult(new CampaignDetail
        {
            Id = Guid.NewGuid(),
            Title = "Channel KBBL",
            Description = "Simpsons Radio Station",
            EstimatedCost = 23456.78,
            Contacts = ["Homer Simpson", "Guy Incognito"]
        });
    }
}
