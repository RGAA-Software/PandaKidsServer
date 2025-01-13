using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     includes one or more books; a suit of books
/// </summary>
public class BookSuit : Entity
{
    public string Content = "";

    public string Details = "";
    
    // automatically added
    public string BookSuitPath = "";

    public string Series = "";
}