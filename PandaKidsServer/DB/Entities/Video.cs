using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     single video
/// </summary>
public class Video : Entity
{
    public string VideoSuitId = "";
}