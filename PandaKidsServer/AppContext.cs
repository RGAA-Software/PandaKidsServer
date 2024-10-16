using PandaKidsServer.DB;
using PandaKidsServer.OnlineUser;
using Serilog;

namespace PandaKidsServer;

public class AppContext
{
    private readonly OnlineUserManager _onlineUserManager;
    private readonly Database _database;
    
    public AppContext()
    {
        _onlineUserManager = new OnlineUserManager(this);
        _database = new Database(this);
    }

    public void Init()
    {
        if (!_database.Connect("mongodb://localhost:27017"))
        {
            Log.Error("Connect to mongodb failed!");
            return;
        }
    }

    public OnlineUserManager GetOnlineUserManager()
    {
        return _onlineUserManager;
    }

    public Database GetDatabase()
    {
        return _database;
    }
}