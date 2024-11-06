using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     single book with or without audio/video
/// </summary>
public class Book : Entity
{
    public string Content = "";

    public string Details = "";

    [BsonIgnore]
    public List<Audio> Audios = [];
    
    [BsonIgnore]
    public List<Video> Videos = [];
    
    public string BookSuitId = "";
}