using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     single book with or without audio/video
/// </summary>
public class Book : Entity
{
    public string Content = "";

    public string Details = "";

    // audio's object id
    public List<string> AudioIds = [];

    [BsonIgnore]
    public List<Audio> Audios = [];
    
    // video's object id
    public List<string> VideoIds = [];
    
    [BsonIgnore]
    public List<Video> Videos = [];
    
    public string BookSuitId = "";
}