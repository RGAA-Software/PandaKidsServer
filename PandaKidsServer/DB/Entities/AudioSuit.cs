using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

public class AudioSuit : Entity
{
    public string Content = "";

    public string Details = "";

    public string AudioSuitPath = "";
}