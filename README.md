# Data Transfer Objects (DTOs) in ASP.NET Core

When building applications, especially with layered architectures or microservices, **Data Transfer Objects (DTOs)** play a critical role in efficiently transferring data between layers or services. This guide demonstrates how to implement DTOs in a RESTful API using **ASP.NET Core**.

---

## Table of Contents

1. [What is a DTO?](#what-is-a-dto)
2. [Purpose of DTOs](#purpose-of-dtos)
3. [Setup](#setup)
4. [Implementation Steps](#implementation-steps)
    - [Step 1: Domain Model](#step-1-domain-model)
    - [Step 2: Creating the DTO](#step-2-creating-the-dto)
    - [Step 3: Mapper Class](#step-3-mapper-class)
    - [Step 4: Repository Interface](#step-4-repository-interface)
    - [Step 5: Implementing the Repository](#step-5-implementing-the-repository)
    - [Step 6: Controller Implementation](#step-6-controller-implementation)
    - [Step 7: Database Configuration](#step-7-database-configuration)
    - [Step 8: Program.cs Configuration](#step-8-programcs-configuration)
5. [Summary](#summary)

---

## What is a DTOs?

A **Data Transfer Objects (DTOs)** is a simple object designed to carry data between layers or services. DTOs contain no business logic and are used to isolate application layers.

### Benefits of DTOs:
- Enforces separation of concerns.
- Improves application performance by transferring only the necessary data.
- Simplifies maintenance and enhances extensibility.

---

## Purpose of DTOs

DTOs encapsulate only the data needed for communication, without including methods or logic. They are particularly useful in:
- Distributed systems (e.g., APIs, microservices).
- Reducing over-fetching or under-fetching of data.

---

## Setup

Before implementing DTOs, ensure your project is configured with the necessary dependencies:
```bash
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package MongoDB.Driver
dotnet add package Swashbuckle.AspNetCore
```

---

## Implementation Steps

### Step 1: Domain Model
Define the **Product** domain model representing the database structure.

```csharp
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CatalogApi.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("name")] public string Name { get; set; } = null!;
    public required decimal Price { get; set; }
    public required string Category { get; set; } = null!;
    public required string Description { get; set; } = null!;
}
```

### Step 2: Creating the DTO
Create a **ProductDto** to transfer product data between layers.

```csharp
namespace CatalogApi.DTO;

public record ProductDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
}
```

### Step 3: Mapper Class
Define a **ProductMapper** to convert between the domain model and the DTO.

```csharp
public static class ProductMapper
{
    public static ProductDto ToDto(Product product) =>
        new ProductDto { Id = product.Id, Name = product.Name, Price = product.Price, Category = product.Category, Description = product.Description };

    public static Product ToModel(ProductDto productDto) =>
        new Product { Id = productDto.Id, Name = productDto.Name, Price = productDto.Price, Category = productDto.Category, Description = productDto.Description };
}
```

### Step 4: Repository Interface
Define methods for CRUD operations on **ProductDto**.

```csharp
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
```

### Step 5: Implementing the Repository
Implement the repository to handle database operations.

```csharp
public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<ProductDto>> GetProductsAsync() =>
        (await _context.Products.Find(_ => true).ToListAsync()).Select(ProductMapper.ToDto).ToList();

    public async Task<ProductDto?> GetProductByIdAsync(string id)
    {
        var product = await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        return product == null ? null : ProductMapper.ToDto(product);
    }

    public async Task CreateProductAsync(ProductDto productDto) =>
        await _context.Products.InsertOneAsync(ProductMapper.ToModel(productDto));

    public async Task<bool> UpdateProductAsync(ProductDto productDto)
    {
        var result = await _context.Products.ReplaceOneAsync(p => p.Id == productDto.Id, ProductMapper.ToModel(productDto));
        return result.IsAcknowledged;
    }

    public async Task<bool> RemoveProductAsync(string id)
    {
        var result = await _context.Products.DeleteOneAsync(p => p.Id == id);
        return result.IsAcknowledged;
    }
}
```

### Step 6: Controller Implementation
Interact with the repository and DTO in the controller.

```csharp
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
    public async Task<List<ProductDto>> GetProductsAsync() => await _repository.GetProductsAsync();

    [HttpGet("{id:length(24)}", Name = "GetProductByIdAsync")]
    public async Task<IActionResult> GetProductByIdAsync(string id)
    {
        var product = await _repository.GetProductByIdAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync(ProductDto productDto)
    {
        await _repository.CreateProductAsync(productDto);
        return CreatedAtRoute("GetProductByIdAsync", new { id = productDto.Id }, productDto);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> UpdateProductAsync(ProductDto productDto) =>
        Ok(await _repository.UpdateProductAsync(productDto));

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> DeleteProductAsync(string id) =>
        Ok(await _repository.RemoveProductAsync(id));
}
```

### Step 7: Database Configuration
Set up MongoDB connection in `appsettings.json`:

```json
{
  "DatabaseSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "ProductDb",
    "CollectionName": "Products"
  }
}
```

### Step 8: Program.cs Configuration
Configure services and middleware.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.Run();
```

---

## Summary

Using **DTOs** ensures:
- Cleaner separation between layers.
- Reduced data transfer size.
- Easier maintainability and scalability.

Enjoy building efficient APIs with **ASP.NET Core** and **DTOs**!

