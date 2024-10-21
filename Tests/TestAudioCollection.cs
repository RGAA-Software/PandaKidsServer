using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;

namespace Tests;

using PandaKidsServer;

public class TestAudioCollection
{
    private AppContext _appContext;
    private AudioOperator _audioOperator;
    
    [SetUp]
    public void Setup()
    {
        _appContext = new AppContext();
        _appContext.Init();
        
        var db = _appContext.GetDatabase();
        _audioOperator = db.GetAudioOperator();
    }

    [Test]
    public void TestInputAudio()
    {
        for (var i = 0; i < 10; i++)
        {
            _audioOperator.InsertEntity(new Audio
            {
                Name = "The Pancake = " + i
            });
        }
    }

    [Test]
    public void TestUpdateAudio()
    {
        var result = _audioOperator.UpdateEntity("6712127e4d166c8612632de4", new Dictionary<string, object>
        {
            {"Name", "Jack Sparrow2"},
        });
        //Assert.That(result, Is.True);
    }

    [Test]
    public void TestDeleteAudio()
    {
        _audioOperator.DeleteEntity("6712127f4d166c8612632de5");
    }

    [Test]
    public void TestFindAAudio()
    {
        var audio = _audioOperator.FindEntityById("6712127e4d166c8612632de4");
        Console.WriteLine("audio: " + audio);
        Assert.NotNull(audio);
    }

    [Test]
    public void TestCountAudio()
    {
        var counts = _audioOperator.CountTotalEntities();
        Console.WriteLine("counts: " + counts);
        Assert.GreaterOrEqual(counts, 1);
    }

    [Test]
    public void TestQueryAudio()
    {
        var audios =_audioOperator.QueryEntities(2, 3);
        Assert.NotNull(audios);
        foreach (var audio in audios)
        {
            Console.WriteLine("audio: " + audio.ToJson());
        }
    }
}