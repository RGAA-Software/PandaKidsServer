using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class UserOperator : CollectionOperator<User>
{
    public UserOperator(AppContext ctx, IMongoCollection<User> collection) : base(ctx, collection)
    {
    }
}