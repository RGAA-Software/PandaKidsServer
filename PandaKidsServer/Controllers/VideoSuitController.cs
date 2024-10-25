using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using Serilog;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;
namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/videosuit")]
public class VideoSuitController(AppContext ctx) : PkBaseController(ctx)
{

    [HttpPost("insert")]
    public async Task<IActionResult> InsertVideoSuit(IFormCollection form) {
        // name
        var name = GetFormValue(form, EntityKey.KeyName);
        // summary
        var summary = GetFormValue(form, EntityKey.KeySummary);
        // details
        var details = GetFormValue(form, EntityKey.KeyDetails);

        if (IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var videoSuit = VideoSuitOp.FindEntityByName(name!);
        
        // cover
        BasicPath? coverPath = null;
        var ret = await CopyImage(form, EntityKey.KeyCoverFile);
        if (ret.Val != null) {
            coverPath = ret.Val!;
            // insert to db
            var entity = ImageOp.InsertEntityIfNotExistByFile(new Image {
                Name = coverPath.Name,
                File = coverPath.RefPath,
            });
            if (entity != null) {
                coverPath.Extra = entity.GetId();
            }
        }
        
        // add new one
        if (videoSuit == null) {
            var newVideoSuit = new VideoSuit {
                Name = name!,
                Summary = summary ?? "",
                Details = details ?? "",
                Cover = coverPath != null ? coverPath!.RefPath : "",
                CoverId = coverPath is { Extra: not null } ? coverPath.Extra! : "",
            };

            if (VideoSuitOp.InsertEntity(newVideoSuit) == null) {
                return RespError(ControllerError.ErrInsertToDbFailed);
            }

            return RespOk();
        }
        else {
            // update exists one
            videoSuit.Name = name!;
            if (summary != null) {
                videoSuit.Summary = summary;
            }
            if (details != null) {
                videoSuit.Details = details;
            }
            if (coverPath != null) {
                videoSuit.Cover = coverPath.RefPath;
                if (coverPath.Extra != null) {
                    videoSuit.CoverId = coverPath.Extra!;
                }
            }
            
            if (!VideoSuitOp.ReplaceEntity(videoSuit)) {
                return RespError(ControllerError.ErrReplaceInDbFailed);
            }
            return RespOk();
        }
    }

    [HttpPost("delete")]
    public IActionResult DeleteVideoSuit(IFormCollection form) {
        string? id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return VideoSuitOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }
    
    [HttpPost("add/video")]
    public async Task<IActionResult> AddVideo(IFormCollection form) {
        var videoFiles = FilterVideoFiles(form);
        if (videoFiles.Count <= 0) {
            return RespError(ControllerError.ErrNoFile);
        }
        
        var id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        var videoSuit = VideoSuitOp.FindEntityById(id!);
        if (videoSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }

        var videoPaths = new List<BasicPath>();
        foreach (var videoFile in videoFiles) {
            var ret = await CopyVideo(videoFile);
            if (ret.Key == null && ret.Val != null) {
                var audioPath = ret.Val!;
                videoPaths.Add(audioPath);
                // insert into db
                var entity = AudioOp.InsertEntityIfNotExistByFile(new Audio {
                    Name = audioPath.Name,
                    File = audioPath.RefPath,
                });
                if (entity == null) {
                    Log.Error("Insert " + audioPath.Name + " to db failed.");
                    continue;
                }
                audioPath.Extra = entity.GetId();
            }
        }
        
        foreach (var videoPath in videoPaths) {
            if (videoPath.Extra != null) {
                var audioId = videoPath.Extra!;
                if (!videoSuit.VideoIds.Contains(audioId)) {
                    videoSuit.VideoIds.Add(audioId);
                }
            }
        }

        if (!VideoSuitOp.ReplaceEntity(videoSuit)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }
        return RespOk();
    }

    [HttpPost("delete/video")]
    public IActionResult DeleteVideo(IFormCollection form) {
        var entityId = GetFormValue(form, EntityKey.KeyId);
        var videoId = GetFormValue(form, EntityKey.KeyVideoId);
        if (IsEmpty(entityId) || IsEmpty(videoId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var videoSuit = VideoSuitOp.FindEntityById(entityId!);
        if (videoSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }

        videoSuit.VideoIds.Remove(videoId!);
        if (!VideoSuitOp.ReplaceEntity(videoSuit)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }

        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryVideoSuits() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var videoSuits = VideoSuitOp.QueryEntities(page, pageSize);
        FillInVideoSuit(videoSuits);
        return RespOkData(EntityKey.RespVideoSuits, videoSuits);
    }
    
    
    [HttpGet("query/like/name")]
    public IActionResult QueryVideosLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var videoSuits = VideoSuitOp.QueryEntitiesLikeName(name!, page, pageSize);
        FillInVideoSuit(videoSuits);
        return RespOkData(EntityKey.RespVideoSuits, videoSuits);
    }
    
    [HttpPost("add/categories")]
    public IActionResult AddCategories(IFormCollection form) {
        return RespOk();
    }
    
    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }
    
    private void FillInVideoSuit(List<VideoSuit>? videoSuits) {
        if (videoSuits == null) {
            return;
        }
        foreach (var videoSuit in videoSuits) {
            foreach (var videoId in videoSuit.VideoIds) {
                if (!IsValidId(videoId)) {
                    continue;
                }
                var video = VideoOp.FindEntityById(videoId);
                if (video != null) {
                    videoSuit.Videos.Add(video);
                }
            }
        }
    }
}