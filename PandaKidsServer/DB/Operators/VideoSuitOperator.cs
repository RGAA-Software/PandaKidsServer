using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class VideoSuitOperator : CollectionOperator<VideoSuit>
{
    public VideoSuitOperator(AppContext ctx, IMongoCollection<VideoSuit> collection) : base(ctx, collection) {
    }
}