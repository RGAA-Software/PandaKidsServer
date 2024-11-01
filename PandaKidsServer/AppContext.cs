using MongoDB.Driver;
using Nett;
using PandaKidsServer.Common;
using PandaKidsServer.DB;
using PandaKidsServer.OnlineUser;
using PandaKidsServer.ResManager;
using Serilog;

namespace PandaKidsServer;

public class AppContext
{
    private Database _database;
    private OnlineUserManager _onlineUserManager;
    private ResManager.ResManager _resManager;
    private PresetResManager _presetResManager;
    private readonly Settings _settings;
    
    public AppContext() {
        var settingsPath = Directory.GetCurrentDirectory() + "/settings.toml";
        _settings = Toml.ReadFile(settingsPath).Get<Settings>(); 
        _settings.CheckParams();
        Log.Information("settings: " + _settings.Dump());
        StartClearThread();
    }

    public void Init() {
        _database = new Database(this);
        if (!_database.Connect("mongodb://localhost:27017")) {
            Log.Error("Connect to mongodb failed!");
            return;
        }
        _onlineUserManager = new OnlineUserManager(this);
        _resManager = new ResManager.ResManager(this);
        _resManager.Init();
        _presetResManager = new PresetResManager(this);
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

    private void StartClearThread() {
        new Thread(() => {
            for (;;) {
                Thread.Sleep(1000 * 600);
                MemoryClean.ClearMemory();
            }
        }).Start();
    }
}