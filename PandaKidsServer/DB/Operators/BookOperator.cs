using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class BookOperator : CollectionOperator<Book>
{
    public List<Book> QueryEntities(string bookSuitId, int page, int pageSize) {
        var filter = Builders<Book>.Filter.Eq(EntityKey.KeyBookSuitId, bookSuitId);
        var docs = Collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList();
        return docs;
    }
}