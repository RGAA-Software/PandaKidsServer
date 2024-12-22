using MongoDB.Driver;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using Serilog;

namespace PandaKidsServer.DB;

public class Database
{
    private readonly AppContext _appContext;

    private AudioOperator _audioOperator;
    private AudioSuitOperator _audioSuitOperator;
    private BookOperator _bookOperator;
    private BookSuitOperator _bookSuitOperator;
    private ImageOperator _imageOperator;
    private ImageSuitOperator _imageSuitOperator;
    private MongoClient _mongoClient;
    private IMongoDatabase _mongoDatabase;
    private UserOperator _userOperator;
    private VideoOperator _videoOperator;
    private VideoSuitOperator _videoSuitOperator;
    private SeriesOperator _seriesOperator;

    public Database(AppContext ctx) {
        _appContext = ctx;
    }

    private TEntityOperator? GetOperator<TE, TEntityOperator>() 
        where TE : Entity  
        where TEntityOperator : CollectionOperator<TE>, new() {
        var entityName = typeof(TE).Name;
        var c = _mongoDatabase.GetCollection<TE>(entityName);
        if (c == null) {
            _mongoDatabase.CreateCollection(entityName);
            c = _mongoDatabase.GetCollection<TE>(entityName);
        }

        if (c == null) {
            return null;
        }
        var entityOperator =  new TEntityOperator();
        entityOperator.SetAppContext(_appContext);
        entityOperator.SetCollection(c);
        return entityOperator;
    }

    public bool Connect(string url) {
        try {
            _mongoClient = new MongoClient(url);
            _mongoDatabase = _mongoClient.GetDatabase("PandaKids");
            // _mongoDatabase.CreateCollection(nameof(Audio));
            // _mongoDatabase.CreateCollection(nameof(AudioSuit));
            // _mongoDatabase.CreateCollection(nameof(Book));
            // _mongoDatabase.CreateCollection(nameof(BookSuit));
            // _mongoDatabase.CreateCollection(nameof(User));
            // _mongoDatabase.CreateCollection(nameof(Video));
            // _mongoDatabase.CreateCollection(nameof(VideoSuit));
            // _mongoDatabase.CreateCollection(nameof(Image));
            // _mongoDatabase.CreateCollection(nameof(ImageSuit));
            // _mongoDatabase.CreateCollection(nameof(Series));
        }
        catch (Exception e) {
            Log.Error("Connect to mongodb failed: " + e);
            return false;
        }

        //var audioCollection = _mongoDatabase.GetCollection<Audio>(nameof(Audio));
        _audioOperator = GetOperator<Audio, AudioOperator>()!;//new AudioOperator(_appContext, audioCollection);

        //var audioSuitCollection = _mongoDatabase.GetCollection<AudioSuit>(nameof(AudioSuit));
        _audioSuitOperator = GetOperator<AudioSuit, AudioSuitOperator>()!;//new AudioSuitOperator(_appContext, audioSuitCollection);

        //var bookCollection = _mongoDatabase.GetCollection<Book>(nameof(Book));
        _bookOperator = GetOperator<Book, BookOperator>()!;//new BookOperator(_appContext, bookCollection);

        //var bookSuitCollection = _mongoDatabase.GetCollection<BookSuit>(nameof(BookSuit));
        _bookSuitOperator = GetOperator<BookSuit, BookSuitOperator>()!;//new BookSuitOperator(_appContext, bookSuitCollection);

        //var userCollection = _mongoDatabase.GetCollection<User>(nameof(User));
        _userOperator = GetOperator<User, UserOperator>()!;//new UserOperator(_appContext, userCollection);

        //var videoCollection = _mongoDatabase.GetCollection<Video>(nameof(Video));
        _videoOperator = GetOperator<Video, VideoOperator>()!;//new VideoOperator(_appContext, videoCollection);

        //var videoSuitCollection = _mongoDatabase.GetCollection<VideoSuit>(nameof(VideoSuit));
        _videoSuitOperator = GetOperator<VideoSuit, VideoSuitOperator>()!;//new VideoSuitOperator(_appContext, videoSuitCollection);

        //var imageCollection = _mongoDatabase.GetCollection<Image>(nameof(Image));
        _imageOperator = GetOperator<Image, ImageOperator>()!;//new ImageOperator(_appContext, imageCollection);

        //var imageSuitCollection = _mongoDatabase.GetCollection<ImageSuit>(nameof(ImageSuit));
        _imageSuitOperator = GetOperator<ImageSuit, ImageSuitOperator>()!;//new ImageSuitOperator(_appContext, imageSuitCollection);

        //var seriesCollection = _mongoDatabase.GetCollection<Series>(nameof(Series));
        _seriesOperator = GetOperator<Series, SeriesOperator>()!;//new SeriesOperator(_appContext, seriesCollection);
        
        return true;
    }

    public AudioOperator GetAudioOperator() {
        return _audioOperator;
    }

    public AudioSuitOperator GetAudioSuitOperator() {
        return _audioSuitOperator;
    }

    public BookOperator GetBookOperator() {
        return _bookOperator;
    }

    public BookSuitOperator GetBookSuitOperator() {
        return _bookSuitOperator;
    }

    public UserOperator GetUserOperator() {
        return _userOperator;
    }

    public VideoOperator GetVideoOperator() {
        return _videoOperator;
    }

    public VideoSuitOperator GetVideoSuitOperator() {
        return _videoSuitOperator;
    }

    public ImageOperator GetImageOperator() {
        return _imageOperator;
    }

    public ImageSuitOperator GetImageSuitOperator() {
        return _imageSuitOperator;
    }

    public SeriesOperator GetSeriesOperator() {
        return _seriesOperator;
    }
}