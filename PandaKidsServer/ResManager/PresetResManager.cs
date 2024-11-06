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
        // traverse the whole Preset folder
        TraverseDirectory(presetPath, folderPath => {
            var configPath = "";
            var configCoverPath = "";
            // search for a config file
            Traverse1LevelFiles(folderPath, filePath => {
                var fileName = GetFileName(filePath);
                if (fileName == ConfigFile) {
                    configPath = filePath;    
                } 
                else if (fileName is ConfigCoverJpg or ConfigCoverPng) {
                    configCoverPath = filePath;
                    Console.WriteLine($"coverPath: {configCoverPath}");
                }

                if (!IsEmpty(configPath) && !IsEmpty(configCoverPath)) {
                    return true;
                }
                return false;
            });
            
            // found cover file
            Image? imageEntity = null;
            if (!IsEmpty(configCoverPath)) {
                var imageOp = _appContext.GetDatabase().GetImageOperator();
                var resPath = _appContext.GetSettings().ResPath;
                if (configCoverPath.Contains(resPath)) {
                    var refCoverPath = configCoverPath[(resPath.Length+1)..];
                    Console.WriteLine("==>CoverPath: " + refCoverPath);
                    imageEntity = imageOp.FindEntityByFilePath(refCoverPath);
                    if (imageEntity == null) {
                        imageEntity = imageOp.InsertEntity(new Image {
                            File = refCoverPath,
                        });
                    }
                    else {
                        imageEntity.File = refCoverPath;
                        imageOp.ReplaceEntity(imageEntity);
                    }
                }
            }
            
            // found config file
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
                    // this is a suit of videos
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
                            Grades = config.Grades,
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
                        videoSuitEntity.Grades = config.Grades;
                        op.ReplaceEntity(videoSuitEntity);
                    }
                } 
                else if (config.SuitType == SuitAudio) {
                    // a suit of audios
                    var op = _appContext.GetDatabase().GetAudioSuitOperator();
                    suitEntity = op.FindEntityByAudioSuitPath(folderPath);
                    if (suitEntity == null) {
                        suitEntity = op.InsertEntity(new AudioSuit {
                            Cover = imageEntity != null ? imageEntity.File : "",
                            CoverId = imageEntity != null ? imageEntity.Id.ToString() : "",
                            Name = config.SuitName,
                            Summary = config.SuitSummary,
                            Author = config.SuitAuthor,
                            Categories = config.SuitCategories,
                            AudioSuitPath = folderPath,
                            Grades = config.Grades,
                        });
                    }
                    else {
                        var audioSuitEntity = (AudioSuit)suitEntity;
                        audioSuitEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        audioSuitEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        audioSuitEntity.Name = config.SuitName;
                        audioSuitEntity.Summary = config.SuitSummary;
                        audioSuitEntity.Author = config.SuitAuthor;
                        audioSuitEntity.Categories = config.SuitCategories;
                        audioSuitEntity.AudioSuitPath = folderPath;
                        audioSuitEntity.Grades = config.Grades;
                        op.ReplaceEntity(audioSuitEntity);
                    }
                } 
                else if (config.SuitType == SuitBook) {
                    // a suit of books
                    var op = _appContext.GetDatabase().GetBookSuitOperator();
                    suitEntity = op.FindEntityByBookSuitPath(folderPath);
                    if (suitEntity == null) {
                        suitEntity = op.InsertEntity(new BookSuit {
                            Cover = imageEntity != null ? imageEntity.File : "",
                            CoverId = imageEntity != null ? imageEntity.Id.ToString() : "",
                            Name = config.SuitName,
                            Summary = config.SuitSummary,
                            Author = config.SuitAuthor,
                            Categories = config.SuitCategories,
                            BookSuitPath = folderPath,
                            Grades = config.Grades,
                        });
                    }
                    else {
                        var bookSuitEntity = (BookSuit)suitEntity;
                        bookSuitEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        bookSuitEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        bookSuitEntity.Name = config.SuitName;
                        bookSuitEntity.Summary = config.SuitSummary;
                        bookSuitEntity.Author = config.SuitAuthor;
                        bookSuitEntity.Categories = config.SuitCategories;
                        bookSuitEntity.BookSuitPath = folderPath;
                        bookSuitEntity.Grades = config.Grades;
                        op.ReplaceEntity(bookSuitEntity);
                    }
                } 
                else if (config.SuitType == SuitImage) {
                    // a suit of images
                    var op = _appContext.GetDatabase().GetImageSuitOperator();
                    suitEntity = op.FindEntityByImageSuitPath(folderPath);
                    if (suitEntity == null) {
                        suitEntity = op.InsertEntity(new ImageSuit {
                            Cover = imageEntity != null ? imageEntity.File : "",
                            CoverId = imageEntity != null ? imageEntity.Id.ToString() : "",
                            Name = config.SuitName,
                            Summary = config.SuitSummary,
                            Author = config.SuitAuthor,
                            Categories = config.SuitCategories,
                            ImageSuitPath = folderPath,
                            Grades = config.Grades,
                        });
                    }
                    else {
                        var imageSuitEntity = (ImageSuit)suitEntity;
                        imageSuitEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        imageSuitEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        imageSuitEntity.Name = config.SuitName;
                        imageSuitEntity.Summary = config.SuitSummary;
                        imageSuitEntity.Author = config.SuitAuthor;
                        imageSuitEntity.Categories = config.SuitCategories;
                        imageSuitEntity.ImageSuitPath = folderPath;
                        imageSuitEntity.Grades = config.Grades;
                        op.ReplaceEntity(imageSuitEntity);
                    }
                }

                if (suitEntity == null) {
                    Console.WriteLine("Insert suit failed: " + config);
                    return false;
                }

                Traverse1LevelFiles(folderPath, filePath => {
                    if (filePath == configPath) {
                        return false;
                    }

                    var extension = GetFileExtension(filePath).ToLower();
                    if (config.SuitType == SuitVideo && extension == ".mp4") {
                        // video file
                        var videoOp = _appContext.GetDatabase().GetVideoOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        //Console.WriteLine("refPath: " + refPath);

                        var videoEntity = videoOp.FindEntityByFilePath(refPath);
                        if (videoEntity == null) {
                            videoOp.InsertEntity(new Video {
                                Name = GetFileNameWithoutExtension(filePath),
                                VideoSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                                Grades = config.Grades,
                            });
                        }
                        else {
                            // todo: update it
                            videoEntity.VideoSuitId = suitEntity.Id.ToString();
                            videoEntity.Name = GetFileNameWithoutExtension(filePath);
                            videoEntity.File = refPath;
                            videoEntity.Grades = config.Grades;
                            videoOp.ReplaceEntity(videoEntity);
                        }
                    } 
                    else if (config.SuitType == SuitAudio && extension == ".mp3") {
                        // audio. mp3 file
                        var audioOp = _appContext.GetDatabase().GetAudioOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        //Console.WriteLine("refPath: " + refPath);

                        var audioEntity = audioOp.FindEntityByFilePath(refPath);
                        if (audioEntity == null) {
                            audioOp.InsertEntity(new Audio {
                                Name = GetFileNameWithoutExtension(filePath),
                                AudioSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                                Grades = config.Grades,
                            });
                        }
                        else {
                            // todo: update it
                            audioEntity.AudioSuitId = suitEntity.Id.ToString();
                            audioEntity.Name = GetFileNameWithoutExtension(filePath);
                            audioEntity.File = refPath;
                            audioEntity.Grades = config.Grades;
                            audioOp.ReplaceEntity(audioEntity);
                        }
                    } 
                    else if (config.SuitType == SuitBook && extension == ".pdf") {
                        // book. pdf file
                        var bookOp = _appContext.GetDatabase().GetBookOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        //Console.WriteLine("refPath: " + refPath);

                        var bookEntity = bookOp.FindEntityByFilePath(refPath);
                        if (bookEntity == null) {
                            bookOp.InsertEntity(new Book {
                                Name = GetFileNameWithoutExtension(filePath),
                                BookSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                                Grades = config.Grades,
                            });
                        }
                        else {
                            // todo: update it
                            bookEntity.BookSuitId = suitEntity.Id.ToString();
                            bookEntity.Name = GetFileNameWithoutExtension(filePath);
                            bookEntity.File = refPath;
                            bookEntity.Grades = config.Grades;
                            bookOp.ReplaceEntity(bookEntity);
                        }
                    } 
                    else if (config.SuitType == SuitImage && (extension is ".jpg" or ".jpeg" or ".png")) {
                        // video file
                        var imageOp = _appContext.GetDatabase().GetImageOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        //Console.WriteLine("refPath: " + refPath);

                        var imgEntity = imageOp.FindEntityByFilePath(refPath);
                        if (imgEntity == null) {
                            imageOp.InsertEntity(new Image {
                                Name = GetFileNameWithoutExtension(filePath),
                                ImageSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                                Grades = config.Grades,
                            });
                        }
                        else {
                            imgEntity.ImageSuitId = suitEntity.Id.ToString();
                            imgEntity.Name = GetFileNameWithoutExtension(filePath);
                            imgEntity.File = refPath;
                            imgEntity.Grades = config.Grades;
                            imageOp.ReplaceEntity(imgEntity);
                        }
                    }
                    
                    return false;
                });    
            }

            return false;
        });
        return true;
    }
}