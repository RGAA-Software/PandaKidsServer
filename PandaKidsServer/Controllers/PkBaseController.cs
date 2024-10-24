using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.DB;
using PandaKidsServer.DB.Entities;
using PandaKidsServer.DB.Operators;
using static PandaKidsServer.Common.BasicType;

namespace PandaKidsServer.Controllers;

public class PkBaseController : ControllerBase
{
    protected AppContext AppContext;
    protected readonly Database Database;
    protected readonly ResManager.ResManager ResManager;
    protected readonly BookOperator BookOp;
    protected readonly BookSuitOperator BookSuitOp;
    protected readonly VideoOperator VideoOp;
    protected readonly AudioOperator AudioOp;
    protected readonly ImageOperator ImageOp;
    protected readonly AudioSuitOperator AudioSuitOp;
    
    public PkBaseController(AppContext ctx) {
        AppContext = ctx;
        ResManager = AppContext.GetResManager();
        Database = AppContext.GetDatabase();
        BookOp = Database.GetBookOperator();
        VideoOp = Database.GetVideoOperator();
        AudioOp = Database.GetAudioOperator();
        ImageOp = Database.GetImageOperator();
        AudioSuitOp = Database.GetAudioSuitOperator();
        BookSuitOp = Database.GetBookSuitOperator();
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

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyBook(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicPair<IActionResult?, BasicPath?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToBooksPath(file);
        if (targetPath == null)
            return new BasicPair<IActionResult?, BasicPath?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicPath(targetPath, Path.Combine(ResManager.GetBookRefPath(), file.FileName), 
            Path.GetFileNameWithoutExtension(targetPath));
        return new BasicPair<IActionResult?, BasicPath?>(null, paths);
    }

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyImage(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null)
            return new BasicPair<IActionResult?, BasicPath?>(RespError(ControllerError.ErrNoFile, key),
                null);
        var targetPath = await ResManager.CopyToImagesPath(file);
        if (targetPath == null)
            return new BasicPair<IActionResult?, BasicPath?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicPath(targetPath, Path.Combine(ResManager.GetImageRefPath(), file.FileName), 
            Path.GetFileNameWithoutExtension(targetPath));
        return new BasicPair<IActionResult?, BasicPath?>(null, paths);
    }

    protected List<IFormFile> FilterVideoFiles(IFormCollection form) {
        var result = new List<IFormFile>();
        foreach (var formFile in form.Files) {
            if (formFile.Name.Contains(EntityKey.KeyVideoFile)) {
                result.Add(formFile);
            }
        }
        return result;
    }

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyVideo(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null) {
            return new BasicPair<IActionResult?, BasicPath?>(RespError(ControllerError.ErrNoFile, key),
                null);
        }
        return await CopyVideo(file);
    }

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyVideo(IFormFile file) {
        var targetPath = await ResManager.CopyToVideosPath(file);
        if (targetPath == null)
            return new BasicPair<IActionResult?, BasicPath?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicPath(targetPath, Path.Combine(ResManager.GetVideoRefPath(), file.FileName), 
            Path.GetFileNameWithoutExtension(targetPath));
        return new BasicPair<IActionResult?, BasicPath?>(null, paths);
    }

    protected List<IFormFile> FilterAudioFiles(IFormCollection form) {
        var result = new List<IFormFile>();
        foreach (var formFile in form.Files) {
            if (formFile.Name.Contains(EntityKey.KeyAudioFile)) {
                result.Add(formFile);
            }
        }
        return result;
    }

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyAudio(IFormCollection form, string key) {
        var file = form.Files.GetFile(key);
        if (file == null) {
            return new BasicPair<IActionResult?, BasicPath?>(RespError(ControllerError.ErrNoFile, key),
                null);
        }
        return await CopyAudio(file);
    }

    protected async Task<BasicPair<IActionResult?, BasicPath?>> CopyAudio(IFormFile file) {
        var targetPath = await ResManager.CopyToAudiosPath(file);
        if (targetPath == null)
            return new BasicPair<IActionResult?, BasicPath?>(
                RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        var paths = new BasicPath(targetPath, Path.Combine(ResManager.GetAudioRefPath(), file.FileName), 
            Path.GetFileNameWithoutExtension(targetPath));
        return new BasicPair<IActionResult?, BasicPath?>(null, paths);
    }

    protected string? GetFormValue(IFormCollection form, string key) {
        return form.TryGetValue(key, out var value) ? value.ToString() : null;
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