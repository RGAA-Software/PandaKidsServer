using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PandaKidsServer.DB.Entities;

public abstract class Entity
{
    [BsonElement("_id")]
    public ObjectId Id = ObjectId.GenerateNewId();

    public string Name = "";

    public string Cover = "Cover";

    public string Summary = "Summary";

    public string Author = "Author";
    
    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }

}