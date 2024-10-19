using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
/// single image
/// </summary>
public class Image : Entity
{
    public string ImagePath = "";
}