namespace BlueModusDemo.Models;

public record ProductCategory
{
    public Guid Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public IEnumerable<ProductDetail> Products { get; set; } = Enumerable.Empty<ProductDetail>();
}
