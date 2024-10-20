using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class ImageOperator : CollectionOperator<Image>
{
    public ImageOperator(AppContext ctx, IMongoCollection<Image> collection) : base(ctx, collection) {
    }
}