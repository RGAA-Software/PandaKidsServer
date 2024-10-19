using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class BookSuitOperator : CollectionOperator<BookSuit>
{
    public BookSuitOperator(AppContext ctx, IMongoCollection<BookSuit> collection) : base(ctx, collection)
    {
    }
}