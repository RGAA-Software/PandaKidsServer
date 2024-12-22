using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class ImageSuitOperator : CollectionOperator<ImageSuit>
{
    public ImageSuit? FindEntityByImageSuitPath(string path) {
        return Collection.Find(Builders<ImageSuit>.Filter.Eq(EntityKey.KeyImageSuitPath, path)).FirstOrDefault();
    }
}