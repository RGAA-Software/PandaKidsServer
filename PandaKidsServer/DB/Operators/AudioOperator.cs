using MongoDB.Bson;
using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class AudioOperator
{
    private readonly AppContext _appContext;
    private readonly IMongoCollection<Audio> _collection;
    
    public AudioOperator(AppContext ctx, IMongoCollection<Audio> collection)
    {
        _appContext = ctx;
        _collection = collection;
        var indexKeysDefinition = Builders<Audio>.IndexKeys.Ascending("Eid");
        var createIndexModel = new CreateIndexModel<Audio>(indexKeysDefinition, new CreateIndexOptions { Unique = true });
        var indexName = collection.Indexes.CreateOne(createIndexModel);
    }

    public bool InsertAudio(Audio audio)
    {
        try
        {
            _collection.InsertOne(audio);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public bool Update(Audio audio)
    {
        return false;
    }

    public Audio? FindAudioById(string id)
    {
        var filter = Builders<Audio>.Filter;
        return _collection.Find(filter.Eq("Eid", id)).FirstOrDefault();
    }

}