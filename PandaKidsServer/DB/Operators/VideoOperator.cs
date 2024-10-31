using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class VideoOperator : CollectionOperator<Video>
{
    public VideoOperator(AppContext ctx, IMongoCollection<Video> collection) : base(ctx, collection) {
    }
    
    public List<Video> QueryEntities(string videoSuitId, int page, int pageSize) {
        var filter = Builders<Video>.Filter.Eq(EntityKey.KeyVideoSuitId, videoSuitId);
        var docs = Collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList();
        return docs;
    }
}