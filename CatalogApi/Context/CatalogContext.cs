using CatalogApi.Models;
using MongoDB.Driver;

namespace CatalogApi.Context;

public class CatalogContext : ICatalogContext
{
    public CatalogContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
        var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));

        Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
    }

    public IMongoCollection<Product> Products { get; }
}