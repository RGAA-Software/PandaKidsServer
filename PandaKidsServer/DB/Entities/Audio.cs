using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
/// single audio
/// </summary>
public class Audio : Entity
{
    public override string ToString()
    {
        return "Eid: " + Id.ToString() + ", Name: " + Name;
    }
    
}