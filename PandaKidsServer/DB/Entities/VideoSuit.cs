using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     a suit of video
/// </summary>
public class VideoSuit : Entity
{
    public string Content = "";

    public string Details = "";

    public List<string> VideoIds = [];
    
    [BsonIgnore]
    public List<Video> Videos = [];
}