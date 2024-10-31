using PandaKidsServer.ResManager;

namespace Tests;
using AppContext = PandaKidsServer.AppContext;

public class TestPresetResource
{
    private AppContext _appContext;
    private PresetResManager _presetResManager;

    [SetUp]
    public void Setup() {
        _appContext = new AppContext();
        _appContext.Init();
        _presetResManager = _appContext.GetPresetResManager();
    }

    [Test]
    public void LoadPresetResources() {
        _presetResManager.ReloadAllResources();
    }

}