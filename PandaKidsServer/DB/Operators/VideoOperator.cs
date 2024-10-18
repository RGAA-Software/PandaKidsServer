using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class VideoOperator : CollectionOperator<Video>
{
    public VideoOperator(AppContext ctx, IMongoCollection<Video> collection) : base(ctx, collection)
    {
    }
}