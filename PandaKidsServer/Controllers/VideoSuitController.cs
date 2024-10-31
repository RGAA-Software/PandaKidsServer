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
    public IActionResult AddVideo(IFormCollection form) {
        var videoSuitId = GetFormValue(form, EntityKey.KeyId);
        var videoId = GetFormValue(form, EntityKey.KeyVideoId);
        if (IsEmpty(videoSuitId) || IsEmpty(videoId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var videoSuit = VideoSuitOp.FindEntityById(videoSuitId!);
        if (videoSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "VideoSuit:" + videoSuitId);
        }
        var video = VideoOp.FindEntityById(videoId!);
        if (video == null) {
            return RespError(ControllerError.ErrNoRecordInDb, "Video:" + videoId);
        }
        
        video.VideoSuitId = videoSuitId!;
        if (!VideoOp.ReplaceEntity(video)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }
        return RespOk();
    }

    [HttpPost("delete/video")]
    public IActionResult DeleteVideo(IFormCollection form) {
        var videoId = GetFormValue(form, EntityKey.KeyVideoId);
        if (IsEmpty(videoId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var video = VideoOp.FindEntityById(videoId!);
        if (video == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }

        video.VideoSuitId = "";
        if (!VideoOp.ReplaceEntity(video)) {
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
}