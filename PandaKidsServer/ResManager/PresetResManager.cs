using Nett;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.ResManager;

public class PresetResManager
{
    private const string BaseInfoName = "info.toml";
    private const string CoverName = "cover"; // jpg or png
    private readonly string _presetPath;
    private readonly AppContext _appContext;
    
    public PresetResManager(AppContext ctx) {
        _appContext = ctx;
        _presetPath = ctx.GetResManager().GetPresetPath();
    }

    private PresetBaseInfo? LoadPresetBaseInfo(string folderPath) {
        var baseInfoPath = Path.Combine(folderPath, BaseInfoName);
        return Toml.ReadFile(baseInfoPath).Get<PresetBaseInfo>(); 
    }

    public async Task<bool> ReloadAllResources() {
        ReloadAudios();
        ReloadVideos();
        ReloadBooks();
        ReloadImages();
        return true;
    }

    public async Task<bool> ReloadBooks() {

        return true;
    }

    public async Task<bool> ReloadVideos() {
        var presetPath = _appContext.GetResManager().GetPresetPath();
        Traverse1LevelDirectories(presetPath, resPath => {
            Traverse1LevelFiles(resPath, filePath => {
                
            });
        });
        return true;
    }

    public async Task<bool> ReloadAudios() {
        return true;
    }

    public async Task<bool> ReloadImages() {
        return true;
    }
}