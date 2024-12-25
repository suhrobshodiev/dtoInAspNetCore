using CatalogApi.Models;

namespace CatalogApi.DTO;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Description = product.Description
        };
    }
    
    public static Product ToModel(ProductDto productDto)
    {
        return new Product
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Price = productDto.Price,
            Category = productDto.Category,
            Description = productDto.Description
        };
    }
}