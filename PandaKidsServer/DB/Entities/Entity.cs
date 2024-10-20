using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PandaKidsServer.DB.Entities;

public abstract class Entity
{
    [BsonElement("_id")] 
    public ObjectId Id = ObjectId.GenerateNewId();
    
    public string Author = "";

    public string CoverPath = "";

    public string Name = "";

    public string Summary = "";

    public string FilePath = "";
    
    public string ToJson() {
        return JsonConvert.SerializeObject(this);
    }
}