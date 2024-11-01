using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using Nett;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.ResManager;

public class PresetResManager
{
    private const string ConfigPrefix = "0001";
    private const string ConfigFile = ConfigPrefix + ".toml";
    private const string ConfigCoverJpg = ConfigPrefix + ".jpg";
    private const string ConfigCoverPng = ConfigPrefix + ".png";
    private const string SuitVideo = "video";
    private const string SuitAudio = "audio";
    private const string SuitBook = "book";
    private const string SuitImage = "image";
    
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
            var configPath = "";
            var configCoverPath = "";
            Traverse1LevelFiles(folderPath, filePath => {
                var fileName = GetFileName(filePath);
                if (fileName == ConfigFile) {
                    configPath = filePath;    
                } 
                else if (fileName == ConfigCoverJpg || fileName == ConfigCoverPng) {
                    configCoverPath = filePath;
                }

                if (!IsEmpty(configPath) && !IsEmpty(configCoverPath)) {
                    return true;
                }
                return false;
            });

            Image? imageEntity = null;
            if (!IsEmpty(configCoverPath)) {
                var imageOp = _appContext.GetDatabase().GetImageOperator();
                var resPath = _appContext.GetSettings().ResPath;
                if (configCoverPath.Contains(resPath)) {
                    var refCoverPath = configCoverPath[(resPath.Length+1)..];
                    Console.WriteLine("CoverPath: " + refCoverPath);
                    imageEntity = imageOp.FindEntityByFilePath(refCoverPath);
                    if (imageEntity == null) {
                        imageOp.InsertEntity(new Image {
                            File = refCoverPath,
                        });
                    }
                    else {
                        imageEntity.File = refCoverPath;
                        imageOp.ReplaceEntity(imageEntity);
                    }
                }
            }

            if (!IsEmpty(configPath)) {
                Console.WriteLine("Found Folder: " + folderPath);
                Console.WriteLine("Found base info config : " + configPath);
                var config = LoadPresetBaseInfo(configPath);
                if (config == null) {
                    return false;
                }
                Console.WriteLine(config);
                // 1. insert suit
                Entity? suitEntity = null;
                if (config.SuitType == SuitVideo) {
                    var op = _appContext.GetDatabase().GetVideoSuitOperator();
                    suitEntity = op.FindEntityByVideoSuitPath(folderPath);
                    if (suitEntity == null) {
                        suitEntity = op.InsertEntity(new VideoSuit {
                            Cover = imageEntity != null ? imageEntity.File : "",
                            CoverId = imageEntity != null ? imageEntity.Id.ToString() : "",
                            Name = config.SuitName,
                            Summary = config.SuitSummary,
                            Author = config.SuitAuthor,
                            Categories = config.SuitCategories,
                            VideoSuitPath = folderPath,
                        });
                    }
                    else {
                        var videoSuitEntity = (VideoSuit)suitEntity;
                        videoSuitEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        videoSuitEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        videoSuitEntity.Name = config.SuitName;
                        videoSuitEntity.Summary = config.SuitSummary;
                        videoSuitEntity.Author = config.SuitAuthor;
                        videoSuitEntity.Categories = config.SuitCategories;
                        videoSuitEntity.VideoSuitPath = folderPath;
                        op.ReplaceEntity(videoSuitEntity);
                    }
                } 
                else if (config.SuitType == SuitAudio) {
                    
                } 
                else if (config.SuitType == SuitBook) {
                    
                } 
                else if (config.SuitType == SuitImage) {
                    
                }

                if (suitEntity == null) {
                    Console.WriteLine("Insert suit failed: " + config);
                    return false;
                }

                Traverse1LevelFiles(folderPath, filePath => {
                    if (filePath == configPath) {
                        return false;
                    }
                    
                    if (config.SuitType == SuitVideo) {
                        var videoOp = _appContext.GetDatabase().GetVideoOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        Console.WriteLine("refPath: " + refPath);

                        var videoEntity = videoOp.FindEntityByFilePath(refPath);
                        if (videoEntity == null) {
                            videoOp.InsertEntity(new Video {
                                Name = GetFileNameWithoutExtension(filePath),
                                VideoSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                            });
                        }
                        else {
                            // todo: update it
                            videoEntity.VideoSuitId = suitEntity.Id.ToString();
                            videoEntity.Name = GetFileNameWithoutExtension(filePath);
                            videoEntity.File = refPath;
                            videoOp.ReplaceEntity(videoEntity);
                        }
                    } 
                    else if (config.SuitType == SuitAudio) {
                    
                    } 
                    else if (config.SuitType == SuitBook) {
                    
                    } 
                    else if (config.SuitType == SuitImage) {
                    
                    }
                    
                    return false;
                });    
            }

            return false;
        });
        return true;
    }
}