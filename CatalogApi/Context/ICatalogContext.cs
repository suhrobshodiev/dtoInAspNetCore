using CatalogApi.Models;
using MongoDB.Driver;

namespace CatalogApi.Context;

public interface ICatalogContext
{
    IMongoCollection<Product> Products { get; }
}