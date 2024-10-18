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
        // make dirs
        var currentDirectory = Directory.GetCurrentDirectory() + "/Resources";
        var dirs = new List<string>
        {
            "Books", "Videos", "Audios"
        };
        foreach (var dir in dirs)
        {
            var targetDir = currentDirectory + "/" + dir;
            try
            {
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
            }
            catch (Exception e)
            {
                Log.Error("Create folder failed: " + dir);
            }
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