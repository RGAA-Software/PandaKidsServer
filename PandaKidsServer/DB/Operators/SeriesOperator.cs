using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class SeriesOperator : CollectionOperator<Series>
{
    public SeriesOperator(AppContext ctx, IMongoCollection<Series> collection) : base(ctx, collection) {
    }
}