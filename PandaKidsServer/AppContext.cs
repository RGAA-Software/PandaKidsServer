using MongoDB.Driver;
using Nett;
using PandaKidsServer.DB;
using PandaKidsServer.OnlineUser;
using PandaKidsServer.ResManager;
using Serilog;

namespace PandaKidsServer;

public class AppContext
{
    private readonly Database _database;
    private readonly OnlineUserManager _onlineUserManager;
    private readonly ResManager.ResManager _resManager;
    private readonly PresetResManager _presetResManager;
    private readonly Settings _settings;
    
    public AppContext() {
        var settingsPath = Directory.GetCurrentDirectory() + "/settings.toml";
        _settings = Toml.ReadFile(settingsPath).Get<Settings>(); 
        _settings.CheckParams();
        
        Log.Information("settings: " + _settings.Dump());
        
        _onlineUserManager = new OnlineUserManager(this);
        _database = new Database(this);
        _resManager = new ResManager.ResManager(this);
        _presetResManager = new PresetResManager(this);
    }

    public void Init() {
        if (!_database.Connect("mongodb://localhost:27017")) {
            Log.Error("Connect to mongodb failed!");
            return;
        }

        _resManager.Init();
    }

    public Settings GetSettings() {
        return _settings;
    }
    
    public OnlineUserManager GetOnlineUserManager() {
        return _onlineUserManager;
    }

    public Database GetDatabase() {
        return _database;
    }

    public ResManager.ResManager GetResManager() {
        return _resManager;
    }

    public PresetResManager GetPresetResManager() {
        return _presetResManager;
    }
}