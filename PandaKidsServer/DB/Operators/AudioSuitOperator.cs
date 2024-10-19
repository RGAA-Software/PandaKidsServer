using MongoDB.Bson;
using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class AudioSuitOperator : CollectionOperator<AudioSuit>
{
    public AudioSuitOperator(AppContext ctx, IMongoCollection<AudioSuit> collection) : base(ctx, collection)
    {
    }
}