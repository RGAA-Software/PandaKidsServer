using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class ComicOperator : CollectionOperator<BookSuit>
{
    public ComicOperator(AppContext ctx, IMongoCollection<BookSuit> collection) : base(ctx, collection)
    {
    }
}