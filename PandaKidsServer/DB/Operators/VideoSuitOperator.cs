using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class VideoSuitOperator : CollectionOperator<VideoSuit>
{
    public VideoSuit? FindEntityByVideoSuitPath(string path) {
        return Collection.Find(Builders<VideoSuit>.Filter.Eq(EntityKey.KeyVideoSuitPath, path)).FirstOrDefault();
    }
}