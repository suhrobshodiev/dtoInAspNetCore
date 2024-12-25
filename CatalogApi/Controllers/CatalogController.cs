using CatalogApi.DTO;
using CatalogApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CatalogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly IProductRepository _repository;

    public CatalogController(IProductRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    [HttpGet]
    public async Task<List<ProductDto>> GetProductsAsync()
    {
        return await _repository.GetProductsAsync();
    }

    [HttpGet("{id:length(24)}", Name = "GetProductByIdAsync")]
    public async Task<IActionResult> GetProductByIdAsync(string id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(ProductDto productDto)
    {
        await _repository.CreateProductAsync(productDto);
        return CreatedAtRoute("GetProductByIdAsync", new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateProductAsync(ProductDto productDto)
    {
        return Ok(await _repository.UpdateProductAsync(productDto));
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteProductAsync(string id)
    {
        return Ok(await _repository.RemoveProductAsync(id));
    }
}