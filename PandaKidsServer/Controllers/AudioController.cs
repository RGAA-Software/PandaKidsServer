using Microsoft.AspNetCore.Mvc;
using PandaKidsServer.Common;
using PandaKidsServer.DB.Entities;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;
namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/audio")]
public class AudioController(AppContext appContext) : PkBaseController(appContext)  {

    [HttpPost("insert")]
    public async Task<IActionResult> InsertAudio(IFormCollection form) {
        var inFile = form.Files.GetFile(EntityKey.KeyFile);
        if (inFile != null) {
            var inFilePath = Path.Combine(ResManager.GetAudioAbsPath(), inFile.FileName);
            if (System.IO.File.Exists(inFilePath)) {
                //return RespError(ControllerError.ErrFileAlreadyExist);
            }
        }

        // * audio
        BasicPath audioPath;
        {
            var ret = await CopyAudio(form, EntityKey.KeyFile);
            if (ret.Key != null) return ret.Key;
            audioPath = ret.Val!;
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

        var audio = new Audio {
            Cover = coverPath != null ? coverPath.RefPath : "",
            CoverId = coverPath != null ? coverPath.Extra! : "",
            Name = name ?? audioPath.Name,
            Summary = summary ?? "",
            File = audioPath.RefPath,
        };
        
        var existAudio = AudioOp.FindEntityByFilePath(audio.File);
        if (existAudio != null) {
            return RespError(ControllerError.ErrRecordAlreadyExist);
        }
        if (AudioOp.InsertEntity(audio) == null) {
            return RespError(ControllerError.ErrInsertAudioFailed);
        }
        return RespOkData(EntityKey.RespAudio, audio);
    }

    [HttpPost("delete")]
    public IActionResult DeleteAudio(IFormCollection form) {
        var id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return AudioOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAudio(IFormCollection form) {
        var id = GetFormValue(form, EntityKey.KeyId);
        var name = GetFormValue(form, EntityKey.KeyName);
        var summary = GetFormValue(form, EntityKey.KeySummary);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var audio = AudioOp.FindEntityById(id!);
        if (audio == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }
        if (name != null) {
            audio.Name = name;
        }
        if (summary != null) {
            audio.Summary = summary;
        }
        // * cover
        BasicPath? coverPath = null;
        var retCopyImage = await CopyImage(form, EntityKey.KeyCoverFile);
        if (retCopyImage.Key == null && retCopyImage.Val != null) {
            coverPath = retCopyImage.Val!;
            var image = ImageOp.FindEntityById(audio.CoverId);
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
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), audio.Cover);
                DeleteFile(oldImagePath);
                
                image.Name = coverPath.Name;
                image.File = coverPath.RefPath;
                ImageOp.ReplaceEntity(image);
                coverPath.Extra = audio.CoverId;
            }
        }
        if (coverPath != null) {
            audio.Cover = coverPath.RefPath;
            audio.CoverId = coverPath.Extra!;
        }

        // * audio
        var retCopyAudio = await CopyAudio(form, EntityKey.KeyFile);
        if (retCopyAudio.Key == null && retCopyAudio.Val != null) {
            var audioPath = retCopyAudio.Val!;
            // delete old one
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), audio.File);
            DeleteFile(oldFilePath);
            audio.File = audioPath.RefPath;
        }

        if (!AudioOp.ReplaceEntity(audio)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }

        // categories
        // todo
        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryAudios() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }
        var audios = AudioOp.QueryEntities(page, pageSize);
        FillInAudios(audios);
        return RespOkData(EntityKey.RespAudios, audios);
    }

    [HttpGet("query/like/name")]
    public IActionResult QueryAudiosLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }
        var audios = AudioOp.QueryEntitiesLikeName(name!, page, pageSize);
        FillInAudios(audios);
        return RespOkData(EntityKey.RespAudios, audios);
    }

    [HttpPost("add/categories")]
    public IActionResult AddCategories(IFormCollection form) {
        return RespOk();
    }
    
    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }

    private void FillInAudios(List<Audio>? audios) {
        if (audios == null) {
            return;
        }
        foreach (var audio in audios) {
            
        }
    }
}