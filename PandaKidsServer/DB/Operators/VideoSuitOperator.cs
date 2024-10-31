using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class VideoSuitOperator : CollectionOperator<VideoSuit>
{
    public VideoSuitOperator(AppContext ctx, IMongoCollection<VideoSuit> collection) : base(ctx, collection) {
    }
    
    public VideoSuit? FindEntityByVideoSuitPath(string path) {
        return Collection.Find(Builders<VideoSuit>.Filter.Eq(EntityKey.KeyVideoSuitPath, path)).FirstOrDefault();
    }
}