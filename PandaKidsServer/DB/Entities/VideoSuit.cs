﻿using MongoDB.Bson.Serialization.Attributes;

namespace PandaKidsServer.DB.Entities;

/// <summary>
///     a suit of video
/// </summary>
public class VideoSuit : Entity
{
    public string Content = "";

    public string Details = "";

    // automatically added
    public string VideoSuitPath = "";
    
    [BsonIgnore]
    public List<Video> Videos = [];
}