using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace PandaKidsServer.DB.Entities;

public abstract class Entity
{
    [BsonElement("_id")] 
    public ObjectId Id = ObjectId.GenerateNewId();
    
    public string Author = "";

    // Ref path
    public string Cover = "";
    // id
    public string CoverId = "";

    public string Name = "";

    public string Summary = "";

    // Ref path
    public string File = "";

    public List<string> Categories = [];

    // K1 ~ K12
    public List<string> Grades = [];
    
    public string ToJson() {
        return JsonConvert.SerializeObject(this);
    }

    public string GetId() {
        return Id.ToString();
    }
}