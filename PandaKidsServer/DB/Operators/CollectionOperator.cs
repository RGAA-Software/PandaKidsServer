using System.Text.RegularExpressions;
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
        // var indexKeysDefinition = Builders<T>.IndexKeys.Ascending("_id");
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
        var e = FindEntityByFilePath(entity.File);
        return e ?? InsertEntity(entity);
    }

    public bool DeleteEntity(string id) {
        try {
            Collection.DeleteOne(new BsonDocument("_id", BsonObjectId.Create(id)));
            return true;
        }
        catch (Exception e) {
            return false;
        }
    }

    public bool UpdateEntity(string id, Dictionary<string, object> value) {
        var filter = Builders<T>.Filter.Eq("_id", BsonObjectId.Create(id));
        var result = Collection.UpdateOne(filter,
            new BsonDocument("$set", new BsonDocument(value)),
            new UpdateOptions { IsUpsert = true });
        return result.ModifiedCount > 0;
    }

    public bool ReplaceEntity(T entity) {
        var result = Collection.ReplaceOne(Builders<T>.Filter.Eq("_id", BsonObjectId.Create(entity.Id)), entity);
        return result.MatchedCount > 0;
    }

    public List<T> QueryEntities(int page, int pageSize) {
        var filter = Builders<T>.Filter.Empty;
        var docs = Collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToList();
        return docs;
    }

    public List<T> QueryEntitiesLikeName(string name) {
        var regexPattern = new BsonRegularExpression(new Regex(name, RegexOptions.IgnoreCase));
        var filter = Builders<T>.Filter.Regex(x => x.Name, regexPattern);
        var result = Collection.Find(filter).ToList();
        return result;
    }

    public long CountTotalEntities() {
        var filter = Builders<T>.Filter.Empty;
        var totalRecords = Collection.CountDocuments(filter);
        return totalRecords;
    }
    
    public T? FindEntityById(string id) {
        var filter = Builders<T>.Filter;
        return Collection.Find(filter.Eq("_id", BsonObjectId.Create(id))).FirstOrDefault();
    }
    
    public T? FindEntityByFilePath(string path) {
        return Collection.Find(Builders<T>.Filter.Eq(EntityKey.KeyFile, path)).FirstOrDefault();
    }

    public T? FindEntityByName(string name) {
        return Collection.Find(Builders<T>.Filter.Eq(EntityKey.KeyName, name)).FirstOrDefault();
    }
}