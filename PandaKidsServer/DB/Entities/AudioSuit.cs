using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

public class AudioSuit : Entity
{
    public List<string> AudioIds = [];
    
    public string Content = "";

    public string Details = "";
    
    [BsonIgnore]
    public List<Audio> Audios = [];

    public string AudioSuitPath = "";
}