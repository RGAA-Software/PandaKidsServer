using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     single video
/// </summary>
public class Video : Entity
{
    // refer to a video suit
    public string VideoSuitId = "";
    
    // subtitle path
    public string SubtitlePath = "";
}