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

    private AudioOperator _audioOperator;
    private BookOperator _bookOperator;
    private CartoonOperator _cartoonOperator;
    private ComicOperator _comicOperator;
    private UserOperator _userOperator;
    private VideoOperator _videoOperator;
    private ImageOperator _imageOperator;
    
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
            _mongoDatabase.CreateCollection(nameof(BookSuit));
            _mongoDatabase.CreateCollection(nameof(User));
            _mongoDatabase.CreateCollection(nameof(Video));
            _mongoDatabase.CreateCollection(nameof(Image));
        }
        catch (Exception e)
        {
            Log.Error("Connect to mongodb failed: " + e.ToString());
            return false;
        }
        
        var audioCollection = _mongoDatabase.GetCollection<Audio>(nameof(Audio));
        _audioOperator = new AudioOperator(_appContext, audioCollection);
            
        var bookCollection = _mongoDatabase.GetCollection<Book>(nameof(Book));
        _bookOperator = new BookOperator(_appContext, bookCollection);
        
        var cartoonCollection = _mongoDatabase.GetCollection<Cartoon>(nameof(Cartoon));
        _cartoonOperator = new CartoonOperator(_appContext, cartoonCollection);
        
        var comicCollection = _mongoDatabase.GetCollection<BookSuit>(nameof(BookSuit));
        _comicOperator = new ComicOperator(_appContext, comicCollection);
        
        var userCollection = _mongoDatabase.GetCollection<User>(nameof(User));
        _userOperator = new UserOperator(_appContext, userCollection);
        
        var videoCollection = _mongoDatabase.GetCollection<Video>(nameof(Video));
        _videoOperator = new VideoOperator(_appContext, videoCollection);

        var imageCollection = _mongoDatabase.GetCollection<Image>(nameof(Image));
        _imageOperator = new ImageOperator(_appContext, imageCollection);

        return true;
    }

    public AudioOperator GetAudioOperator()
    {
        return _audioOperator;
    }

    public BookOperator GetBookOperator()
    {
        return _bookOperator;
    }

    public CartoonOperator GetCartoonOperator()
    {
        return _cartoonOperator;
    }

    public ComicOperator GetComicOperator()
    {
        return _comicOperator;
    }

    public UserOperator GetUserOperator()
    {
        return _userOperator;
    }

    public VideoOperator GetVideoOperator()
    {
        return _videoOperator;
    }
}