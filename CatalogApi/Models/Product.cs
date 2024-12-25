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