using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PandaKidsServer.Common;
using PandaKidsServer.DB;

namespace PandaKidsServer.Controllers;

public class PkBaseController : ControllerBase
{
    public class RespMessage
    {
        public int Code = 0;
        public string Message = "";
        public Dictionary<string, object> Data = new();
    }

    protected AppContext _appContext;
    protected ResManager.ResManager _resManager;
    protected Database _database;
    
    public PkBaseController(AppContext ctx)
    {
        _appContext = ctx;
        _resManager = _appContext.GetResManager();
        _database = _appContext.GetDatabase();
    }

    private IActionResult ResponseMessage(int code, string? extra, Dictionary<string, object> data)
    {
        return Ok(JsonConvert.SerializeObject(new RespMessage()
        {
            Code = code,
            Message = ControllerError.Error2String(code) + (extra != null?  ", extra: " + extra : ""),
            Data = data,
        }));
    }

    protected IActionResult RespOk()
    {
        return ResponseMessage(ControllerError.Ok, null, new Dictionary<string, object>());
    }

    protected IActionResult RespOkData(Dictionary<string, object> data)
    {
        return ResponseMessage(ControllerError.Ok, null, data);
    }

    protected IActionResult RespOkValue(object value)
    {
        return RespOkData(new Dictionary<string, object>()
        {
            {"Value", value},
        });
    }

    protected IActionResult RespError(int code)
    {
        return ResponseMessage(code, null, new Dictionary<string, object>());
    }
    
    protected IActionResult RespError(int code, string? extra)
    {
        return ResponseMessage(code, extra, new Dictionary<string, object>());
    }

    protected IActionResult RespErrorData(int code, string? extra, Dictionary<string, object> data)
    {
        return ResponseMessage(code, extra, data);
    }
    
    protected IActionResult RespErrValue(int code, string? extra, object? value)
    {
        return RespErrorData(code, extra, new Dictionary<string, object>()
        {
            {"Value", value},
        });
    }

    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyBook(IFormCollection form, string key)
    {
        var file = form.Files.GetFile(key);
        if (file == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key), null);
        }
        var targetPath = await _resManager.CopyToBooksPath(file);
        if (targetPath == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        }
        var paths = new BasicType.Paths(targetPath, Path.Combine(_resManager.GetBookRefPath(), file.FileName));
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }
    
    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyImage(IFormCollection form, string key)
    {
        var file = form.Files.GetFile(key);
        if (file == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key), null);
        }
        var targetPath = await _resManager.CopyToImagesPath(file);
        if (targetPath == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        }
        var paths = new BasicType.Paths(targetPath, Path.Combine(_resManager.GetImageRefPath(), file.FileName));
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }
    
    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyVideo(IFormCollection form, string key)
    {
        var file = form.Files.GetFile(key);
        if (file == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key), null);
        }
        var targetPath = await _resManager.CopyToVideosPath(file);
        if (targetPath == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        }
        var paths = new BasicType.Paths(targetPath, Path.Combine(_resManager.GetVideoRefPath(), file.FileName));
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }
    
    protected async Task<BasicType.Pair<IActionResult?, BasicType.Paths?>> CopyAudio(IFormCollection form, string key)
    {
        var file = form.Files.GetFile(key);
        if (file == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrNoFile, key), null);
        }
        var targetPath = await _resManager.CopyToAudiosPath(file);
        if (targetPath == null)
        {
            return new BasicType.Pair<IActionResult?, BasicType.Paths?>(RespError(ControllerError.ErrCopyFileFailed, targetPath), null);
        }
        var paths = new BasicType.Paths(targetPath, Path.Combine(_resManager.GetAudioRefPath(), file.FileName));
        return new BasicType.Pair<IActionResult?, BasicType.Paths?>(null, paths);
    }
}