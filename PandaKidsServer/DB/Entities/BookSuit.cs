using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
/// includes one or more books; a suit of books
/// </summary>
public class BookSuit : Entity
{
    public string Author = "";

    public string Content = "";

    public string Details = "";
    
    public string Summary = "";
    
    public List<string> BookIds = [];
}