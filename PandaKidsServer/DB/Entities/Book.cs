using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
/// single book with or without audio/video
/// </summary>
public class Book
{
    [BsonElement("_id")]
    public ObjectId ObjectId = ObjectId.GenerateNewId();
    
    public string Eid = "";

    public string Name = "";

    public string Author = "";
}