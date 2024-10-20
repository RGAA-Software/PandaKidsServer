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
    
    // video's object id
    public List<string> VideoIds = [];
}