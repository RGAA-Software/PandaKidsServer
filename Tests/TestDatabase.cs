using PandaKidsServer.DB.Entities;

namespace Tests;

using PandaKidsServer;

public class TestDatabase
{
    private AppContext _appContext;
    
    [SetUp]
    public void Setup()
    {
        _appContext = new AppContext();
        _appContext.Init();
    }

    [Test]
    public void TestInputAudio()
    {
        var db = _appContext.GetDatabase();
        var op = db.GetAudioOperator();
        op.InsertAudio(new Audio
        {
            Eid = "123456",
            Name = "The Pancake"
        });
    }
}