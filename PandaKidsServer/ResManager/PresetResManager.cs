using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using Nett;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.ResManager;

public class PresetResManager
{
    private const string BaseInfoName = "0001.toml";
    private const string CoverName = "cover"; // jpg or png
    private readonly string _presetPath;
    private readonly AppContext _appContext;
    
    public PresetResManager(AppContext ctx) {
        _appContext = ctx;
        _presetPath = ctx.GetResManager().GetPresetPath();
    }

    private PresetBaseInfo? LoadPresetBaseInfo(string folderPath) {
        return Toml.ReadFile(folderPath).Get<PresetBaseInfo>(); 
    }

    public bool ReloadAllResources() {
        var presetPath = _appContext.GetResManager().GetPresetPath();
        TraverseDirectory(presetPath, folderPath => {
            var foundBaseInfoConfig = false;
            var foundBaseInfoConfigPath = "";
            Traverse1LevelFiles(folderPath, filePath => {
                if (GetFileName(filePath) != BaseInfoName) {
                    return false;
                }
                foundBaseInfoConfig = true;
                foundBaseInfoConfigPath = filePath;
                return true;
            });
            
            if (foundBaseInfoConfig) {
                Console.WriteLine("Found Folder: " + folderPath);
                Console.WriteLine("Found base info config : " + foundBaseInfoConfigPath);
                var config = LoadPresetBaseInfo(foundBaseInfoConfigPath);
                if (config == null) {
                    return false;
                }
                Console.WriteLine(config);
                Traverse1LevelFiles(folderPath, filePath => {
                    if (filePath == foundBaseInfoConfigPath) {
                        return false;
                    }
                    Console.WriteLine(filePath);
                    return false;
                });    
            }

            return false;
        });
        return true;
    }
}