using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class BookOperator : CollectionOperator<Book>
{
    public BookOperator(AppContext ctx, IMongoCollection<Book> collection) : base(ctx, collection) {
    }

    public Book? FindBookByPdfPath(string path) {
        return Collection.Find(Builders<Book>.Filter.Eq(EntityKey.KeyPdfPath, path)).FirstOrDefault();
    }
}