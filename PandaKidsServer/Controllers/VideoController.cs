using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;
namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/video")]
public class VideoController(AppContext appContext) : PkBaseController(appContext)  {

    [HttpPost("insert")]
    public async Task<IActionResult> InsertVideo(IFormCollection form) {
        var inFile = form.Files.GetFile(EntityKey.KeyFile);
        if (inFile != null) {
            var inFilePath = Path.Combine(ResManager.GetVideoAbsPath(), inFile.FileName);
            if (System.IO.File.Exists(inFilePath)) {
                //return RespError(ControllerError.ErrFileAlreadyExist);
            }
        }

        // * video
        BasicType.BasicPath videoPath;
        {
            var ret = await CopyVideo(form, EntityKey.KeyFile);
            if (ret.Key != null) return ret.Key;
            videoPath = ret.Val!;
        }
        
        // * cover
        BasicPath? coverPath = null;
        {
            var ret = await CopyImage(form, EntityKey.KeyCoverFile);
            if (ret.Key == null && ret.Val != null) {
                coverPath = ret.Val!;
                // insert to db
                var entity = ImageOp.InsertEntityIfNotExistByFile(new Image {
                    Name = coverPath.Name,
                    File = coverPath.RefPath,
                });
                if (entity == null) {
                    return RespError(ControllerError.ErrFileAlreadyExist);
                }

                coverPath.Extra = entity.GetId();
            }
        }
        
        var name = GetFormValue(form, EntityKey.KeyName);
        var summary = GetFormValue(form, EntityKey.KeySummary);

        var video = new Video {
            Cover = coverPath != null ? coverPath.RefPath : "",
            CoverId = coverPath!= null ? coverPath.Extra! : "",
            Name = name ?? videoPath.Name,
            Summary = summary ?? "",
            File = videoPath.RefPath,
        };
        
        var existVideo = VideoOp.FindEntityByFilePath(video.File);
        if (existVideo != null) {
            return RespError(ControllerError.ErrRecordAlreadyExist);
        }
        if (VideoOp.InsertEntity(video) == null) {
            return RespError(ControllerError.ErrInsertVideoFailed);
        }
        return RespOkData(EntityKey.RespVideo, video);
    }

    [HttpPost("delete")]
    public IActionResult DeleteVideo(IFormCollection form) {
        var id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return VideoOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateVideo(IFormCollection form) {
        var id = GetFormValue(form, EntityKey.KeyId);
        var name = GetFormValue(form, EntityKey.KeyName);
        var summary = GetFormValue(form, EntityKey.KeySummary);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var video = VideoOp.FindEntityById(id!);
        if (video == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }
        if (name != null) {
            video.Name = name;
        }
        if (summary != null) {
            video.Summary = summary;
        }
        // * cover
        BasicPath? coverPath = null;
        var retCopyImage = await CopyImage(form, EntityKey.KeyCoverFile);
        if (retCopyImage.Key == null && retCopyImage.Val != null) {
            coverPath = retCopyImage.Val!;
            var image = ImageOp.FindEntityById(video.CoverId);
            if (image == null) {
                // insert to db
                var entity = ImageOp.InsertEntityIfNotExistByFile(new Image {
                    Name = coverPath.Name,
                    File = coverPath.RefPath,
                });
                if (entity == null) {
                    return RespError(ControllerError.ErrFileAlreadyExist);
                }
                coverPath.Extra = entity.GetId();
            }
            else {
                var oldImagePath = Path.Combine(appContext.GetSettings().ResPath, video.Cover);
                DeleteFile(oldImagePath);
                
                image.Name = coverPath.Name;
                image.File = coverPath.RefPath;
                ImageOp.ReplaceEntity(image);
                coverPath.Extra = video.CoverId;
            }
        }
        if (coverPath != null) {
            video.Cover = coverPath.RefPath;
            video.CoverId = coverPath.Extra!;
        }

        // * video
        var retCopyVideo = await CopyVideo(form, EntityKey.KeyFile);
        if (retCopyVideo.Key == null && retCopyVideo.Val != null) {
            var videoPath = retCopyVideo.Val!;
            // delete old one
            var oldFilePath = Path.Combine(AppCtx.GetSettings().ResPath, video.File);
            DeleteFile(oldFilePath);
            video.File = videoPath.RefPath;
        }

        if (!VideoOp.ReplaceEntity(video)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }

        // categories
        // todo
        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryVideos() {
        string? videoSuitId = Request.Query[EntityKey.KeyVideoSuitId];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }

        List<Video> videos;
        if (!IsEmpty(videoSuitId)) {
            videos = VideoOp.QueryEntities(videoSuitId!, page, pageSize);
        } else {
            videos = VideoOp.QueryEntities(page, pageSize);
        }
        FillInVideos(videos);
        return RespOkData(EntityKey.RespVideos, videos);
    }

    [HttpGet("query/like/name")]
    public IActionResult QueryVideosLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }
        var videos = VideoOp.QueryEntitiesLikeName(name!, page, pageSize);
        FillInVideos(videos);
        return RespOkData(EntityKey.RespVideos, videos);
    }

    [HttpPost("add/categories")]
    public IActionResult AddCategories(IFormCollection form) {
        return RespOk();
    }
    
    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }

    private void FillInVideos(List<Video>? videos) {
        if (videos == null) {
            return;
        }
        foreach (var video in videos) {
            
        }
    }
}