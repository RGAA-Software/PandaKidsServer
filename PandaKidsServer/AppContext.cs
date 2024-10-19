using PandaKidsServer.DB;
using PandaKidsServer.OnlineUser;
using Serilog;

namespace PandaKidsServer;

public class AppContext
{
    private readonly OnlineUserManager _onlineUserManager;
    private readonly Database _database;
    private readonly ResManager.ResManager _resManager;
    
    public AppContext()
    {
        _onlineUserManager = new OnlineUserManager(this);
        _database = new Database(this);
        _resManager = new ResManager.ResManager(this);
    }

    public void Init()
    {
        if (!_database.Connect("mongodb://localhost:27017"))
        {
            Log.Error("Connect to mongodb failed!");
            return;
        }
        _resManager.Init();
    }

    public OnlineUserManager GetOnlineUserManager()
    {
        return _onlineUserManager;
    }

    public Database GetDatabase()
    {
        return _database;
    }

    public ResManager.ResManager GetResManager()
    {
        return _resManager;
    }

}