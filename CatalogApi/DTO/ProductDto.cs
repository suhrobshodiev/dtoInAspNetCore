namespace CatalogApi.DTO;

public record ProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }

}