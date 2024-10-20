using Serilog;

namespace PandaKidsServer.ResManager;

public class ResManager
{
    private const string Level1DirBooks = "Books";
    private const string Level1DirVideos = "Videos";
    private const string Level1DirAudios = "Audios";
    private const string Level1DirImages = "Images";
    private readonly AppContext _appContext;

    private string _basePath = "";

    public ResManager(AppContext ctx) {
        _appContext = ctx;
    }

    public void Init() {
        // make dirs
        _basePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");
        var dirs = new List<string> {
            Level1DirBooks, Level1DirVideos, Level1DirAudios, Level1DirImages
        };
        foreach (var dir in dirs) {
            var targetDir = _basePath + "/" + dir;
            try {
                if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
            }
            catch (Exception e) {
                Log.Error("Create folder failed: " + dir);
            }
        }
    }

    public string GetBookAbsPath() {
        return Path.Combine(_basePath, Level1DirBooks);
    }

    public string GetBookRefPath() {
        return Path.Combine("Resources", Level1DirBooks);
    }

    public string GetVideoAbsPath() {
        return Path.Combine(_basePath, Level1DirVideos);
    }

    public string GetVideoRefPath() {
        return Path.Combine("Resources", Level1DirVideos);
    }

    public string GetAudioAbsPath() {
        return Path.Combine(_basePath, Level1DirAudios);
    }

    public string GetAudioRefPath() {
        return Path.Combine("Resources", Level1DirAudios);
    }

    public string GetImageAbsPath() {
        return Path.Combine(_basePath, Level1DirImages);
    }

    public string GetImageRefPath() {
        return Path.Combine("Resources", Level1DirImages);
    }

    private static async Task<string?> CopyToPath(string path, IFormFile file) {
        try {
            var filePath = Path.Combine(path, file.FileName);
            await using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            await file.CopyToAsync(stream);
            return File.Exists(filePath) ? filePath : null;
        }
        catch (Exception e) {
            return null;
        }
    }

    public async Task<string?> CopyToBooksPath(IFormFile file) {
        return await CopyToPath(GetBookAbsPath(), file);
    }

    public async Task<string?> CopyToImagesPath(IFormFile file) {
        return await CopyToPath(GetImageAbsPath(), file);
    }

    public async Task<string?> CopyToVideosPath(IFormFile file) {
        return await CopyToPath(GetVideoAbsPath(), file);
    }

    public async Task<string?> CopyToAudiosPath(IFormFile file) {
        return await CopyToPath(GetAudioAbsPath(), file);
    }
}