using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using static PandaKidsServer.Common.Common;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.Controllers;

[ApiController]
[Route("pandakids/audio/suit")]
public class AudioSuitController(AppContext ctx) : PkBaseController(ctx)
{
    private readonly AppContext _appContext = ctx;

    [HttpPost("insert")]
    public async Task<IActionResult> InsertAudio(IFormCollection form) {
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
        if (audioSuit != null) {
            return RespError(ControllerError.ErrRecordAlreadyExist);
        }
        
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

    [HttpGet("delete")]
    public IActionResult DeleteAudio() {
        string? id = Request.Query[EntityKey.KeyId];
        if (IsEmpty(id)) {
            return RespError(ControllerError.ErrParamErr);
        }
        return AudioSuitOp.DeleteEntity(id!) ? RespOk() : RespError(ControllerError.ErrDeleteFailed);
    }

    // [HttpGet("query/audio/suit")]
    // public IActionResult QueryAudio() {
    //     return Ok();
    // }
    //
    // [HttpGet("query")]
    // public IActionResult QueryAudios() {
    //     return Ok();
    // }
}