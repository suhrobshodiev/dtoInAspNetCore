using CatalogApi.Context;
using CatalogApi.DTO;
using MongoDB.Driver;

namespace CatalogApi.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ProductDto>> GetProductsAsync()
        {
            var products = await _context.Products.Find(_ => true).ToListAsync();
            return products.Select(ProductMapper.ToDto).ToList();
        }

        public async Task<ProductDto?> GetProductByIdAsync(string id)
        {
            var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null) return null;
            return ProductMapper.ToDto(product);
        }

        public async Task CreateProductAsync(ProductDto productDto)
        {
            var product = ProductMapper.ToModel(productDto);
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProductAsync(ProductDto productDto)
        {
            var product = ProductMapper.ToModel(productDto);
            var result = await _context.Products.ReplaceOneAsync(
                p => p.Id == productDto.Id,
                product
            );
            return result.IsAcknowledged;
        }

        public async Task<bool> RemoveProductAsync(string id)
        {
            var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return result.IsAcknowledged;
        }
    }
}