using MediatR;
using MongoDB.Driver;
using Nett;
using PandaKidsServer.Common;
using PandaKidsServer.DB;
using PandaKidsServer.Messages;
using PandaKidsServer.OnlineUser;
using PandaKidsServer.ResManager;
using Serilog;

namespace PandaKidsServer;

public class AppContext
{
    
    public class Timer5SHandler : INotificationHandler<AppMessages.MsgTimer5S>
    {
        public Task Handle(AppMessages.MsgTimer5S msg, CancellationToken cancellationToken) {
            return Task.FromResult("ok");
        }
    }
    
    private Database _database;
    private OnlineUserManager _onlineUserManager;
    private ResManager.ResManager _resManager;
    private PresetResManager _presetResManager;
    private readonly Settings _settings;
    private System.Threading.Timer _timer;
    
    public AppContext() {
        var settingsPath = Directory.GetCurrentDirectory() + "/settings.toml";
        _settings = Toml.ReadFile(settingsPath).Get<Settings>(); 
        _settings.CheckParams();
        Log.Information("settings: " + _settings.Dump());
    }

    public void Init(IMediator? mediator) {
        _database = new Database(this);
        if (!_database.Connect("mongodb://localhost:27017")) {
            Log.Error("Connect to mongodb failed!");
            return;
        }
        _onlineUserManager = new OnlineUserManager(this);
        _resManager = new ResManager.ResManager(this);
        _resManager.Init();
        _presetResManager = new PresetResManager(this);

        if (mediator != null) {
            _timer = new System.Threading.Timer(state => {
                mediator.Publish(new AppMessages.MsgTimer5S(this));
            }, "5s timer", 0, 5000);
        }

        StartClearThread();
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