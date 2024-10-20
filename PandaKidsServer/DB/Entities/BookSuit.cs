namespace PandaKidsServer.DB.Entities;

/// <summary>
///     includes one or more books; a suit of books
/// </summary>
public class BookSuit : Entity
{
    public List<string> BookIds = [];
    public string Content = "";

    public string Details = "";
}