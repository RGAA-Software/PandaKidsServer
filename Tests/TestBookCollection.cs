using MongoDB.Bson;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using AppContext = PandaKidsServer.AppContext;

namespace Tests;

public class TestBookCollection
{
    private AppContext _appContext;
    private BookOperator _bookOperator;

    [SetUp]
    public void Setup() {
        _appContext = new AppContext();
        _appContext.Init(null);

        var db = _appContext.GetDatabase();
        _bookOperator = db.GetBookOperator();
    }

    [Test]
    public void TestInputBook() {
        for (var i = 0; i < 10; i++)
            _bookOperator.InsertEntity(new Book {
                Name = "Fancy Dress = " + i
            });
    }

    [Test]
    public void TestReplaceBook() {
        var id = "67165f16c658045dc7937f64";
        var book = _bookOperator.FindEntityById(id);
        Assert.NotNull(book);

        book.Author = "Jack Sparrow";
        var ok = _bookOperator.ReplaceEntity(book);
        Assert.True(ok);
    }
}