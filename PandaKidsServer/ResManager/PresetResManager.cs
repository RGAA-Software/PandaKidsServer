using System.Drawing;
using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using MetadataExtractor;
using Nett;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;
using System.Drawing.Imaging;
using Image = PandaKidsServer.DB.Entities.Image;

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
        Console.WriteLine("@ "+ presetPath);
        // traverse the whole Preset folder
        TraverseDirectory(presetPath, folderPath => {
            var configPath = "";
            var configCoverPath = "";
            // search for a config file
            Traverse1LevelFiles(folderPath, filePath => {
                var fileExt = GetFileExtensionLower(filePath);
                var fileName = GetFileName(filePath);
                if (fileName != ConfigFile && fileName != ConfigCoverJpg && fileName != ConfigCoverPng
                    && !IsVideoFile(filePath) && fileExt != ".mp3" && fileExt != ".pdf") {
                    //Console.WriteLine("Ignore the file: " + filePath);
                    return false;
                }

                if (fileName == ConfigFile) {
                    configPath = filePath;    
                } 
                else if (fileName is ConfigCoverJpg or ConfigCoverPng) {
                    configCoverPath = filePath;
                    Console.WriteLine($"coverPath: {configCoverPath}");
                }
                // break this loop if there are no config file and cover
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
                
                // process series
                if (config.Series.Length > 0) {
                    var op = _appContext.GetDatabase().GetSeriesOperator();
                    var entity = op.FindEntityByName(config.Series);
                    if (entity == null) {
                        // insert it
                        entity = op.InsertEntity(new Series {
                            Name = config.Series,
                            Type = config.SuitType,
                        });
                    }
                    else {
                        // update it
                        var se = (Series)entity;
                        se.Name = config.Series;
                        se.Type = config.SuitType;
                        op.ReplaceEntity(se);
                    }
                }

                // 1. insert suit
                Entity? suitEntity = null;
                if (config.SuitType == SuitVideo) {
                    // this is a suit of videos
                    var op = _appContext.GetDatabase().GetVideoSuitOperator();
                    suitEntity = op.FindEntityByVideoSuitPath(folderPath);
                    // can't find this suit, insert one
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
                            Series = config.Series,
                        });
                    }
                    else {
                        // found it, update it.
                        var vsEntity = (VideoSuit)suitEntity;
                        vsEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        vsEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        vsEntity.Name = config.SuitName;
                        vsEntity.Summary = config.SuitSummary;
                        vsEntity.Author = config.SuitAuthor;
                        vsEntity.Categories = config.SuitCategories;
                        vsEntity.VideoSuitPath = folderPath;
                        vsEntity.Grades = config.Grades;
                        vsEntity.Series = config.Series;
                        op.ReplaceEntity(vsEntity);
                    }
                } 
                else if (config.SuitType == SuitAudio) {
                    // a suit of audios
                    var op = _appContext.GetDatabase().GetAudioSuitOperator();
                    suitEntity = op.FindEntityByAudioSuitPath(folderPath);
                    // can't find, insert one
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
                            Series = config.Series,
                        });
                    }
                    else {
                        // found it, update it.
                        var asEntity = (AudioSuit)suitEntity;
                        asEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        asEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        asEntity.Name = config.SuitName;
                        asEntity.Summary = config.SuitSummary;
                        asEntity.Author = config.SuitAuthor;
                        asEntity.Categories = config.SuitCategories;
                        asEntity.AudioSuitPath = folderPath;
                        asEntity.Grades = config.Grades;
                        asEntity.Series = config.Series;
                        op.ReplaceEntity(asEntity);
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
                            Series = config.Series,
                        });
                    }
                    else {
                        var bsEntity = (BookSuit)suitEntity;
                        bsEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        bsEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        bsEntity.Name = config.SuitName;
                        bsEntity.Summary = config.SuitSummary;
                        bsEntity.Author = config.SuitAuthor;
                        bsEntity.Categories = config.SuitCategories;
                        bsEntity.BookSuitPath = folderPath;
                        bsEntity.Grades = config.Grades;
                        bsEntity.Series = config.Series;
                        op.ReplaceEntity(bsEntity);
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
                            Series = config.Series,
                        });
                    }
                    else {
                        var isEntity = (ImageSuit)suitEntity;
                        isEntity.Cover = imageEntity != null ? imageEntity.File : "";
                        isEntity.CoverId = imageEntity != null ? imageEntity.Id.ToString() : "";
                        isEntity.Name = config.SuitName;
                        isEntity.Summary = config.SuitSummary;
                        isEntity.Author = config.SuitAuthor;
                        isEntity.Categories = config.SuitCategories;
                        isEntity.ImageSuitPath = folderPath;
                        isEntity.Grades = config.Grades;
                        isEntity.Series = config.Series;
                        op.ReplaceEntity(isEntity);
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
                    if (config.SuitType == SuitVideo && IsVideoFile(filePath)) {
                        // video file
                        var videoOp = _appContext.GetDatabase().GetVideoOperator();
                        var resPath = _appContext.GetSettings().ResPath;
                        if (!filePath.Contains(resPath)) {
                            Console.WriteLine("--> Error path : " + filePath);
                            return false;
                        }

                        var refPath = filePath.Substring(resPath.Length + 1);
                        //Console.WriteLine("refPath: " + refPath);
                        
                        // generate thumbnail
                        var targetThumbnailPath = "";
                        var thumbnailExists = false;
                        var outputName = GetFileNameWithoutExtension(filePath) + ".png";
                        var outputFolder = GetFolder(filePath);
                        if (outputFolder != null) {
                            outputFolder = outputFolder.Replace("\\", "/");
                            var outputPath = outputFolder + "/" + outputName;
                            if (Exists(outputPath)) {
                                try {
                                    var md = ImageMetadataReader.ReadMetadata(outputPath);
                                    thumbnailExists = true;
                                    targetThumbnailPath = outputPath;
                                }
                                catch (Exception e) {
                                    Console.WriteLine("Get image info failed: " + e.Message);   
                                }
                            }

                            if (!thumbnailExists) {
                                var thumbnailPath = FFmpegHelper.GenerateThumbnail(filePath, outputPath);
                                if (thumbnailPath != null && thumbnailPath.Contains(resPath)) {
                                    targetThumbnailPath = thumbnailPath;
                                    Console.WriteLine("Generate thumbnail: " + thumbnailPath);
                                }
                            }
                        }
                        if (targetThumbnailPath.Contains(resPath)) {
                            // image scale
                            ImageHelper.ZoomPictureAlongWidth(targetThumbnailPath, 480);
                            
                            targetThumbnailPath = targetThumbnailPath[(resPath.Length + 1)..];
                        }
                        
                        var videoEntity = videoOp.FindEntityByFilePath(refPath);
                        if (videoEntity == null) {
                            videoOp.InsertEntity(new Video {
                                Name = GetFileNameWithoutExtension(filePath),
                                VideoSuitId = suitEntity.Id.ToString(),
                                File = refPath,
                                Grades = config.Grades,
                                Cover = targetThumbnailPath,
                            });
                        }
                        else {
                            // todo: update it
                            videoEntity.VideoSuitId = suitEntity.Id.ToString();
                            videoEntity.Name = GetFileNameWithoutExtension(filePath);
                            videoEntity.File = refPath;
                            videoEntity.Grades = config.Grades;
                            videoEntity.Cover = targetThumbnailPath;
                            if (!videoOp.ReplaceEntity(videoEntity)) {
                                Console.WriteLine("Replace entity failed!");
                            }
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

    private List<string> ParseGrades(string grade) {
        var grades = grade.Split(',');
        return [..grades];
    }
}