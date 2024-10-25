using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using Serilog;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/audiosuit")]
public class AudioSuitController(AppContext ctx) : PkBaseController(ctx)
{

    [HttpPost("insert")]
    public async Task<IActionResult> InsertAudioSuit(IFormCollection form) {
        // name
        var name = GetFormValue(form, EntityKey.KeyName);
        // summary
        var summary = GetFormValue(form, EntityKey.KeySummary);
        // details
        var details = GetFormValue(form, EntityKey.KeyDetails);

        if (IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var audioSuit = AudioSuitOp.FindEntityByName(name!);
        
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
        if (audioSuit == null) {
            var newAudioSuit = new AudioSuit {
                Name = name!,
                Summary = summary ?? "",
                Details = details ?? "",
                Cover = coverPath != null ? coverPath!.RefPath : "",
                CoverId = coverPath is { Extra: not null } ? coverPath.Extra! : "",
            };

            if (AudioSuitOp.InsertEntity(newAudioSuit) == null) {
                return RespError(ControllerError.ErrInsertToDbFailed);
            }

            return RespOk();
        }
        else {
            // update exists one
            audioSuit.Name = name!;
            if (summary != null) {
                audioSuit.Summary = summary;
            }
            if (details != null) {
                audioSuit.Details = details;
            }
            if (coverPath != null) {
                audioSuit.Cover = coverPath.RefPath;
                if (coverPath.Extra != null) {
                    audioSuit.CoverId = coverPath.Extra!;
                }
            }
            
            if (!AudioSuitOp.ReplaceEntity(audioSuit)) {
                return RespError(ControllerError.ErrReplaceInDbFailed);
            }
            return RespOk();
        }
    }

    [HttpPost("delete")]
    public IActionResult DeleteAudioSuit(IFormCollection form) {
        string? id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return AudioSuitOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }

    [HttpPost("add/audio")]
    public async Task<IActionResult> AddAudio(IFormCollection form) {
        var audioFiles = FilterAudioFiles(form);
        if (audioFiles.Count <= 0) {
            return RespError(ControllerError.ErrNoFile);
        }
        
        var id = GetFormValue(form, EntityKey.KeyId);
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        var audioSuit = AudioSuitOp.FindEntityById(id!);
        if (audioSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }

        var audioPaths = new List<BasicPath>();
        foreach (var audioFile in audioFiles) {
            var ret = await CopyAudio(audioFile);
            if (ret.Key == null && ret.Val != null) {
                var audioPath = ret.Val!;
                audioPaths.Add(audioPath);
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
        
        foreach (var audioPath in audioPaths) {
            if (audioPath.Extra != null) {
                var audioId = audioPath.Extra!;
                if (!audioSuit.AudioIds.Contains(audioId)) {
                    audioSuit.AudioIds.Add(audioId);
                }
            }
        }

        if (!AudioSuitOp.ReplaceEntity(audioSuit)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }
        return RespOk();
    }

    [HttpPost("delete/audio")]
    public IActionResult DeleteAudio(IFormCollection form) {
        var entityId = GetFormValue(form, EntityKey.KeyId);
        var audioId = GetFormValue(form, EntityKey.KeyAudioId);
        if (IsEmpty(entityId) || IsEmpty(audioId)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var audioSuit = AudioSuitOp.FindEntityById(entityId!);
        if (audioSuit == null) {
            return RespError(ControllerError.ErrNoRecordInDb);
        }

        audioSuit.AudioIds.Remove(audioId!);
        if (!AudioSuitOp.ReplaceEntity(audioSuit)) {
            return RespError(ControllerError.ErrReplaceInDbFailed);
        }

        return RespOk();
    }

    [HttpGet("query")]
    public IActionResult QueryAudioSuits() {
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize)) {
            return RespError(ControllerError.ErrParamErr);
        }

        var audioSuits = AudioSuitOp.QueryEntities(page, pageSize);
        FillInAudioSuits(audioSuits);
        return RespOkData(EntityKey.RespAudioSuits, audioSuits);
    }
    
    [HttpGet("query/like/name")]
    public IActionResult QueryAudioSuitsLikeName() {
        string? name = Request.Query[EntityKey.KeyName];
        int page = AsInt(Request.Query[EntityKey.KeyPage]);
        int pageSize = AsInt(Request.Query[EntityKey.KeyPageSize]);
        if (!IsValidInt(page) || !IsValidInt(pageSize) || IsEmpty(name)) {
            return RespError(ControllerError.ErrParamErr);
        }
        
        var audioSuits = AudioSuitOp.QueryEntitiesLikeName(name!, page, pageSize);
        FillInAudioSuits(audioSuits);
        return RespOkData(EntityKey.RespAudioSuits, audioSuits);
    }

    [HttpPost("add/categories")]
    public IActionResult AddCategories() {
        return Ok();
    }

    [HttpGet("query/category")]
    public IActionResult QueryByCategory() {
        return RespOk();
    }

    private void FillInAudioSuits(List<AudioSuit>? audioSuits) {
        if (audioSuits == null) {
            return;
        }
        foreach (var audioSuit in audioSuits) {
            foreach (var audioId in audioSuit.AudioIds) {
                if (!IsValidId(audioId)) {
                    continue;
                }
                var audio = AudioOp.FindEntityById(audioId);
                if (audio != null) {
                    audioSuit.Audios.Add(audio);
                }
            }
        }
    }
}