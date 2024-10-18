using MongoDB.Bson;
using MongoDB.Driver;

namespace PandaKidsServer.DB.Operators;

public abstract class CollectionOperator<T>
{
    private readonly AppContext _appContext;
    private readonly IMongoCollection<T> _collection;
    
    public CollectionOperator(AppContext ctx, IMongoCollection<T> collection)
    {
        _appContext = ctx;
        _collection = collection;
        var indexKeysDefinition = Builders<T>.IndexKeys.Ascending("Eid");
        var createIndexModel = new CreateIndexModel<T>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        var indexName = collection.Indexes.CreateOne(createIndexModel);
    }

    public bool InsertEntity(T entity)
    {
        try
        {
            _collection.InsertOne(entity);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public void DeleteEntity(string eid)
    {
        _collection.DeleteOne(new BsonDocument("Eid", eid));
    }

    public bool UpdateEntity(string eid, Dictionary<string, object> value)
    {
        var filter = Builders<T>.Filter.Eq("Eid", eid);
        var result = _collection.UpdateOne(filter, 
            new BsonDocument("$set", new BsonDocument(value)), 
            new UpdateOptions { IsUpsert = true });
        return result.ModifiedCount > 0;
    }
    
    public T? FindEntityById(string id)
    {
        var filter = Builders<T>.Filter;
        return _collection.Find(filter.Eq("Eid", id)).FirstOrDefault();
    }

    public List<T> QueryEntity(int page, int pageSize)
    {
        var filter = Builders<T>.Filter.Empty;
        var docs = _collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList<T>();
        return docs;
    }

    public long CountTotalEntities()
    {
        var filter = Builders<T>.Filter.Empty;
        var totalRecords = _collection.CountDocuments(filter);
        return totalRecords;
    }

}