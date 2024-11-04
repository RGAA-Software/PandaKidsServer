﻿using MongoDB.Driver;
using PandaKidsServer.DB.Entities;

namespace PandaKidsServer.DB.Operators;

public class AudioSuitOperator : CollectionOperator<AudioSuit>
{
    public AudioSuitOperator(AppContext ctx, IMongoCollection<AudioSuit> collection) : base(ctx, collection) {
    }
    
    public AudioSuit? FindEntityByAudioSuitPath(string path) {
        return Collection.Find(Builders<AudioSuit>.Filter.Eq(EntityKey.KeyAudioSuitPath, path)).FirstOrDefault();
    }
}