using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class CartoonOperator : CollectionOperator<Cartoon>
{
    public CartoonOperator(AppContext ctx, IMongoCollection<Cartoon> collection) : base(ctx, collection)
    {
    }
}