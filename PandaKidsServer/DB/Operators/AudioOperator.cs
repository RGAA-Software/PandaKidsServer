using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class AudioOperator : CollectionOperator<Audio>
{
    public AudioOperator(AppContext ctx, IMongoCollection<Audio> collection) : base(ctx, collection) {
    }
}