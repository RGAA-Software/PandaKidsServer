using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

public class Audio
{
    [BsonElement("_id")]
    public ObjectId ObjectId = ObjectId.GenerateNewId();
    
    public string Eid = "";

    public string Name = "";
}