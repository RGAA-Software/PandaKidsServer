using MongoDB.Bson;
using MongoDB.Driver;
using PandaKidsServer.DB.Entities;
using Serilog;

namespace PandaKidsServer.DB.Operators;

public abstract class CollectionOperator<T> where T: Entity
{
    private readonly AppContext _appContext;
    private readonly IMongoCollection<T> _collection;
    
    public CollectionOperator(AppContext ctx, IMongoCollection<T> collection)
    {
        _appContext = ctx;
        _collection = collection;
        // var indexKeysDefinition = Builders<T>.IndexKeys.Ascending("Eid");
        // var createIndexModel = new CreateIndexModel<T>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        // var indexName = collection.Indexes.CreateOne(createIndexModel);
    }

    public string InsertEntity(T entity)
    {
        try
        {
            _collection.InsertOne(entity);
            return entity.Id.ToString();
        }
        catch (Exception e)
        {
            Log.Error("Insert error:" + entity + " because of : " + e.Message);
            return "";
        }
    }

    public void DeleteEntity(string id)
    {
        _collection.DeleteOne(new BsonDocument("Eid", BsonObjectId.Create(id)));
    }

    public bool UpdateEntity(string id, Dictionary<string, object> value)
    {
        var filter = Builders<T>.Filter.Eq("Eid", BsonObjectId.Create(id));
        var result = _collection.UpdateOne(filter, 
            new BsonDocument("$set", new BsonDocument(value)), 
            new UpdateOptions { IsUpsert = true });
        return result.ModifiedCount > 0;
    }
    
    public T? FindEntityById(string id)
    {
        var filter = Builders<T>.Filter;
        return _collection.Find(filter.Eq("Eid", BsonObjectId.Create(id))).FirstOrDefault();
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