using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

public class Audio
{
    [BsonElement("_id")]
    public ObjectId ObjectId = ObjectId.GenerateNewId();
    
    public string Eid = "";

    public string Name = "";

    public int Age = 0;

    public string Pets = "";

    public string Hobbies = "";

    public override string ToString()
    {
        return "Eid: " + Eid + ", Name: " + Name;
    }
}