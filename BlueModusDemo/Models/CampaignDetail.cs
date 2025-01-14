namespace BlueModusDemo.Models;

public record CampaignDetail
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double EstimatedCost { get; set; }
    public IEnumerable<string> Contacts { get; set; } = Enumerable.Empty<string>();
}
