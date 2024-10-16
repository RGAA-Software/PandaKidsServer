using MongoDB.Driver;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using Serilog;

namespace PandaKidsServer.DB;

public class Database
{
    private readonly AppContext _appContext;
    private MongoClient _mongoClient;
    private IMongoDatabase _mongoDatabase;
    
    private IMongoCollection<Book> _bookCollection;
    private IMongoCollection<Cartoon> _cartoonCollection;
    private IMongoCollection<Comic> _comicCollection;
    private IMongoCollection<User> _userCollection;
    private IMongoCollection<Video> _videoCollection;

    private AudioOperator _audioOperator;
    
    public Database(AppContext ctx)
    {
        _appContext = ctx;
    }

    public bool Connect(string url)
    {
        try
        {
            _mongoClient = new MongoClient(url);
            _mongoDatabase = _mongoClient.GetDatabase("PandaKids");
            _mongoDatabase.CreateCollection(nameof(Audio));
            _mongoDatabase.CreateCollection(nameof(Book));
            _mongoDatabase.CreateCollection(nameof(Cartoon));
            _mongoDatabase.CreateCollection(nameof(Comic));
            _mongoDatabase.CreateCollection(nameof(User));
            _mongoDatabase.CreateCollection(nameof(Video));
        }
        catch (Exception e)
        {
            Log.Error("Connect to mongodb failed: " + e.ToString());
            return false;
        }
        
        var audioCollection = _mongoDatabase.GetCollection<Audio>(nameof(Audio));
        _audioOperator = new AudioOperator(_appContext, audioCollection);
            
        _bookCollection = _mongoDatabase.GetCollection<Book>(nameof(Book));
        _cartoonCollection = _mongoDatabase.GetCollection<Cartoon>(nameof(Cartoon));
        _comicCollection = _mongoDatabase.GetCollection<Comic>(nameof(Comic));
        _userCollection = _mongoDatabase.GetCollection<User>(nameof(User));
        _videoCollection = _mongoDatabase.GetCollection<Video>(nameof(Video));

        return true;
    }

    public AudioOperator GetAudioOperator()
    {
        return _audioOperator;
    }

}