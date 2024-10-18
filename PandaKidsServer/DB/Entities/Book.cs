using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
/// single book with or without audio/video
/// </summary>
public class Book : Entity
{
    public string Content = "";

    public string Details = "";

    public string PdfPath = "";

    // video's object id
    public List<string> VideoIds = [];
    // audio's object id
    public List<string> AudioIds = [];
}