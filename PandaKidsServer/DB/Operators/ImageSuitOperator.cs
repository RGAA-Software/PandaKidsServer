using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class ImageSuitOperator : CollectionOperator<ImageSuit>
{
    public ImageSuitOperator(AppContext ctx, IMongoCollection<ImageSuit> collection) : base(ctx, collection) {
    }
}