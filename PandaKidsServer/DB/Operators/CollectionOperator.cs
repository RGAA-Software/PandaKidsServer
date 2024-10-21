using MongoDB.Bson;
using MongoDB.Driver;
using PandaKidsServer.DB.Entities;
using Serilog;

namespace PandaKidsServer.DB.Operators;

public abstract class CollectionOperator<T> where T : Entity
{
    protected readonly AppContext AppContext;
    protected readonly IMongoCollection<T> Collection;

    public CollectionOperator(AppContext ctx, IMongoCollection<T> collection) {
        AppContext = ctx;
        Collection = collection;
        // var indexKeysDefinition = Builders<T>.IndexKeys.Ascending("Eid");
        // var createIndexModel = new CreateIndexModel<T>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        // var indexName = collection.Indexes.CreateOne(createIndexModel);
    }

    public T? InsertEntity(T entity) {
        try {
            Collection.InsertOne(entity);
            return entity;
        }
        catch (Exception e) {
            Log.Error("Insert error:" + entity + " because of : " + e.Message);
            return null;
        }
    }

    public T? InsertEntityIfNotExistByFile(T entity) {
        var e = FindFilePath(entity.File);
        return e ?? InsertEntity(entity);
    }

    public void DeleteEntity(string id) {
        Collection.DeleteOne(new BsonDocument("Eid", BsonObjectId.Create(id)));
    }

    public bool UpdateEntity(string id, Dictionary<string, object> value) {
        var filter = Builders<T>.Filter.Eq("Eid", BsonObjectId.Create(id));
        var result = Collection.UpdateOne(filter,
            new BsonDocument("$set", new BsonDocument(value)),
            new UpdateOptions { IsUpsert = true });
        return result.ModifiedCount > 0;
    }

    public T? FindEntityById(string id) {
        var filter = Builders<T>.Filter;
        return Collection.Find(filter.Eq("Eid", BsonObjectId.Create(id))).FirstOrDefault();
    }

    public List<T> QueryEntity(int page, int pageSize) {
        var filter = Builders<T>.Filter.Empty;
        var docs = Collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList();
        return docs;
    }

    public long CountTotalEntities() {
        var filter = Builders<T>.Filter.Empty;
        var totalRecords = Collection.CountDocuments(filter);
        return totalRecords;
    }
    
    public T? FindFilePath(string path) {
        return Collection.Find(Builders<T>.Filter.Eq(EntityKey.KeyFile, path)).FirstOrDefault();
    }
}