namespace PandaKidsServer.ResManager;

public class PresetResManager
{
    private readonly string _presetPath;
    
    public PresetResManager(AppContext ctx) {
        _presetPath = ctx.GetResManager().GetPresetPath();
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
        return true;
    }

    public async Task<bool> ReloadAudios() {
        return true;
    }

    public async Task<bool> ReloadImages() {
        return true;
    }
}