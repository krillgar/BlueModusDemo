using BlueModusDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BlueModusDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [Route("{category}/{subcategory}/{style}")]
    public IActionResult CampaignA(string category, string subcategory, string style)
    {
        var random = new Random();

        return new OkObjectResult(new ProductCategory
        {
            Id = Guid.NewGuid(),
            Category = SentenceCase(category),
            SubCategory = SentenceCase(subcategory),
            Style = SentenceCase(style),
            Products = [
                new ProductDetail
                {
                    Id = random.Next(),
                    Name = "Widget",
                    Count = random.Next(150),
                    Price = random.NextDouble() * random.Next(1750)
                },
                new ProductDetail
                {
                    Id = random.Next(),
                    Name = "Doohickey",
                    Count = random.Next(250),
                    Price = random.NextDouble() * random.Next(82)
                },
                new ProductDetail
                {
                    Id = random.Next(),
                    Name = "Gizmo",
                    Count = random.Next(375),
                    Price = random.NextDouble() * random.Next(97)
                }
            ]
        });

        string SentenceCase(string str)
        {
            var culture = CultureInfo.InvariantCulture;

            var parts = str.Split(' ').Select(s => char.ToUpper(s[0], culture) + s.Substring(1).ToLower(culture));

            return string.Join(' ', parts);
        }
    }
}
