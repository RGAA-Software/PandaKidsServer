using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;

namespace Tests;

using PandaKidsServer;

public class TestBookCollection
{
    private AppContext _appContext;
    private BookOperator _bookOperator;
    
    [SetUp]
    public void Setup()
    {
        _appContext = new AppContext();
        _appContext.Init();
        
        var db = _appContext.GetDatabase();
        _bookOperator = db.GetBookOperator();
    }
    
    [Test]
    public void TestInputBook()
    {
        for (var i = 0; i < 10; i++)
        {
            _bookOperator.InsertEntity(new Book
            {
                Eid = "Book_" + i,
                Name = "Fancy Dress = " + i
            });
        }
    }

}