using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.Common;
using PandaKidsServer.DB;
using PandaKidsServer.DB.Operators;

namespace PandaKidsServer.Controllers;

public class PkBaseController : ControllerBase
{
    protected AppContext AppContext;
    protected readonly Database Database;
    protected readonly ResManager.ResManager ResManager;
    protected readonly BookOperator BookOperator;
    protected readonly VideoOperator VideoOperator;
    protected readonly AudioOperator AudioOperator;
    
    public PkBaseController(AppContext ctx) {
        AppContext = ctx;
        ResManager = AppContext.GetResManager();
        Database = AppContext.GetDatabase();
        BookOperator = Database.GetBookOperator();
        VideoOperator = Database.GetVideoOperator();
        AudioOperator = Database.GetAudioOperator();
    }

    private IActionResult ResponseMessage(int code, string? extra, Dictionary<string, object> data) {
        return Ok(JsonConvert.SerializeObject(new RespMessage {
            Code = code,
            Message = ControllerError.Error2String(code) + (extra != null ? ", extra: " + extra : ""),
            Data = data
        }));
    }

    protected IActionResult RespOk() {
        return ResponseMessage(ControllerError.Ok, null, new Dictionary<string, object>());
    }

    protected IActionResult RespOkData(Dictionary<string, object> data) {
        return ResponseMessage(ControllerError.Ok, null, data);
    }
    
    protected IActionResult RespOkData(string key, object data) {
        return ResponseMessage(ControllerError.Ok, null, MakeRespData(key, data));
    }
    
    protected IActionResult RespOkValue(object value) {
        return RespOkData(new Dictionary<string, object> {
            { "Value", value }
        });
    }

    protected IActionResult RespError(int code) {
        return ResponseMessage(code, null, new Dictionary<string, object>());
    }

    protected IActionResult RespError(int code, string? extra) {
        return ResponseMessage(code, extra, new Dictionary<string, object>());
    }

    protected IActionResult RespErrorData(int code, string? extra, Dictionary<string, object> data) {
        return ResponseMessage(code, extra, data);
    }

    protected IActionResult RespErrValue(int code, string? extra, object? value) {
        return RespErrorData(code, extra, new Dictionary<string, object> {
            { "Value", value }
        });
    }

    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyBook(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToBooksPath(file);
        if (targetPath == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicType.Paths(targetPath, Path.Combine(ResManager.GetBookRefPath(), file.FileName), file.FileName);
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }

    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyImage(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToImagesPath(file);
        if (targetPath == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicType.Paths(targetPath, Path.Combine(ResManager.GetImageRefPath(), file.FileName), file.FileName);
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }

    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyVideo(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToVideosPath(file);
        if (targetPath == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicType.Paths(targetPath, Path.Combine(ResManager.GetVideoRefPath(), file.FileName), file.FileName);
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }

    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyAudio(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToAudiosPath(file);
        if (targetPath == null)
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicType.Paths(targetPath, Path.Combine(ResManager.GetAudioRefPath(), file.FileName), file.FileName);
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }

    protected string GetFormValue(IFormCollection form, string key) {
        return form.TryGetValue(key, out var value) ? value.ToString() : "";
    }

    protected Dictionary<string, object> MakeRespData(string name, object data) {
        return new Dictionary<string, object> {
            { name, data }
        };
    }

    public class RespMessage
    {
        public int Code;
        public Dictionary<string, object> Data = new();
        public string Message = "";
    }
}