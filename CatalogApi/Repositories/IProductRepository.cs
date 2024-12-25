using CatalogApi.DTO;

namespace CatalogApi.Repositories;

public interface IProductRepository
{
    Task<List<ProductDto>> GetProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(string id);
    Task CreateProductAsync(ProductDto productDto);
    Task<bool> UpdateProductAsync(ProductDto productDto);
    Task<bool> RemoveProductAsync(string id);
}