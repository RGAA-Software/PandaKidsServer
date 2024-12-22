using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class AudioSuitOperator : CollectionOperator<AudioSuit>
{
    public AudioSuit? FindEntityByAudioSuitPath(string path) {
        return Collection.Find(Builders<AudioSuit>.Filter.Eq(EntityKey.KeyAudioSuitPath, path)).FirstOrDefault();
    }
}